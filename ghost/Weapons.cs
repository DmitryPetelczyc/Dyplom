using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ghost
{
    static class info
    {
        public static IPAddress MyIP { get; set; }
        public static IPAddress NodeIP { get; set; }
        public static int MyPort { get; set; }
        public static int NodePort { get; set; }
        public static Socket ConSocket { get; set; } //=
            //new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        public static Socket ListnSocket { get; set; } =
            new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

    }
    delegate void ConnectionHandler(string message);
    static class Connection
    {
        static ConnectionHandler _del;
        public static void SetConnectionHandler(ConnectionHandler del)
        {
            _del = del;
        }
        public static void Connect()
        {
            info.ConSocket =
                new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            info.ConSocket.Connect(info.NodeIP, info.NodePort);
            Send.send(info.ConSocket, info.MyIP + ":" + info.MyPort, "0");

            _del?.Invoke("[" + info.NodeIP + ":" + info.NodePort + "] " + Main.Lang[8] + "\n");
        }

        /*
        private static ManualResetEvent connectDone =
            new ManualResetEvent(false);

        internal static void Connect()
        {
            Socket client = info.ConSocket;
            EndPoint remoteEP = new IPEndPoint(info.NodeIP, info.NodePort);
               
            client.BeginConnect(remoteEP, new AsyncCallback(ConnectCallback), client);
        }

        private static void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                Socket client = (Socket)ar.AsyncState;

                client.EndConnect(ar);
                // Signal that the connection has been made.  
                connectDone.Set();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
        */
    }
    class Send
    {
        private static ManualResetEvent sendDone =
            new ManualResetEvent(false);
        internal static void send(Socket client, String data, string code)
        {
            UnicodeEncoding ByteConverter = new UnicodeEncoding();
            byte[] byteData = new byte[256];
            byteData = ByteConverter.GetBytes(code + "`" + data);
            byte[] publicKey = File.ReadAllBytes(@".\public.txt");
            // Begin sending the data to the remote device.  
            //try
            //{
                //MessageBox.Show(""+byteData.Length);
                byteData = rsa.RSAEncrypt(byteData, publicKey, false);

                //MessageBox.Show(""+ByteConverter.GetString(RSADecrypt(byteData, privateKey, false)));
                client.BeginSend(byteData, 0, byteData.Length, 0,
                    new AsyncCallback(SendCallback), client);
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show("" + ex.ToString());
            //}
        }

        /*
        internal static void Noramlsend(Socket client, byte[] data)
        {
            // Convert the string data to byte data using ASCII encoding.  

            // Begin sending the data to the remote device.  
            try
            {
                client.BeginSend(data, 0, data.Length, 0,
                    new AsyncCallback(SendCallback), client);
            }
            catch (Exception ex)
            {
                MessageBox.Show("" + ex.Message);
            }
        }
        */

        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                Socket client = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.  
                int bytesSent = client.EndSend(ar);
                //Console.WriteLine("Sent {0} bytes to server.", bytesSent);

                // Signal that all bytes have been sent.  
                sendDone.Set();
            }
            catch (Exception ex)
            {
                MessageBox.Show("" + ex.Message);
            }
        }
    }
    //Delegate for sending catched message to main form
    delegate void MessageHandler(string message);
    class Listener : Form
    {
        private static ManualResetEvent receiveDone = new ManualResetEvent(false);
        public void StartListening()
        {
            try
            {
                IPEndPoint localEP = new IPEndPoint(info.MyIP, info.MyPort);
                Socket listener = info.ListnSocket;
                //MessageBox.Show($"Local address and port : {localEP.ToString()}");

                try
                {
                    listener.Bind(localEP);
                    listener.Listen(99);

                    while (true)
                    {
                        receiveDone.Reset();

                        listener.BeginAccept(new AsyncCallback(AcceptCallback), listener);

                        receiveDone.WaitOne();
                    }
                }
                catch (ThreadAbortException e) { }
                catch (Exception e)
                {
                    MessageBox.Show("" + e.ToString());
                }
            }
            catch (ThreadAbortException e) { }
            catch (Exception e)
            {
                MessageBox.Show("" + e.ToString());
            }
        }
        public class StateObject
        {
            public Socket workSocket = null;
            public const int BufferSize = 256;
            public byte[] buffer = new byte[BufferSize];
            public StringBuilder sb = new StringBuilder();
        }
        public void AcceptCallback(IAsyncResult ar)
        {
            // Get the socket that handles the client request.  
            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

            // Signal the main thread to continue.  
            receiveDone.Set();

            // Create the state object.  
            StateObject state = new StateObject();
            state.workSocket = handler;
            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                new AsyncCallback(ReadCallback), state);
        }
        MessageHandler _del;

        public void GetMessageHandler(MessageHandler del)
        {
            _del = del;
        }
        public void ReadCallback(IAsyncResult ar)
        {
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;
            byte[] privateKey = File.ReadAllBytes(@".\private.txt");
  
            int read = 0;

            // Read data from the client socket.  
            try { read = handler.EndReceive(ar); }
            catch (System.Net.Sockets.SocketException ex)
            {
                //host disconnect
            }

            // Data was read from the client socket.  
            if (read > 0)
            {
                UnicodeEncoding ByteConverter = new UnicodeEncoding();

                //state.sb.Append(Encoding.Unicode.GetString(state.buffer), 0, read);
                //Decrypting 
                string temp = ByteConverter.GetString(rsa.RSADecrypt(state.buffer, privateKey, false));
                state.sb.Append(temp, 0, temp.Length);

                handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReadCallback), state);
                //Delegate
                _del?.Invoke(state.sb.ToString());


                state.sb.Clear();
            }
            else
            {
                if (state.sb.Length > 1)
                {
                    // All the data has been read from the client;  
                    // display it on the console.  
                    //string content = state.sb.ToString();

                    //_del?.Invoke(Main.Lang[5]);
                    //form1.add("user disconected");
                    //MessageBox.Show($"Read {content.Length} bytes from socket.\n Data : {content}");
                }
                handler.Close();

            }
        }

        /*
        string return_name(string IP)
        {
            string path = @".\file.txt";
            using (FileStream fstream = File.OpenRead(path))
            {
                // преобразуем строку в байты
                byte[] array = new byte[fstream.Length];
                // считываем данные
                fstream.Read(array, 0, array.Length);
                // декодируем байты в строку
                string textFromFile = Encoding.Default.GetString(array);
                string[] str = textFromFile.Split(' ');
                foreach (string ip in str.Reverse())
                {
                    if (ip.Split(':')[0] == IP.Split(':')[0])
                    {
                        return ip.Split(':')[1];
                    }
                }
                return IP;
            }
        }
        */
    }
    static class Weapons
    {
        public static int GetPort()
        {
            var rand = new Random(); int port;
            while (true)
            {
                port = rand.Next(60000);
                if (CheckPort(port))
                {
                    return port;
                }
            }
        }
        public static bool CheckPort(int port)
        {
            IPGlobalProperties igp = IPGlobalProperties.GetIPGlobalProperties();
            TcpConnectionInformation[] tinfo = igp.GetActiveTcpConnections();
            foreach (TcpConnectionInformation tcpi in tinfo)
            {
                if (tcpi.LocalEndPoint.Port == port)
                {
                    return false;
                }
            }
            return true;
        }
        public static string[] GetMyIPs()
        {
            
            IPAddress[] ips = Dns.GetHostByName(Dns.GetHostName()).AddressList;
            string[] result = new string[ips.Length];
            int i = 0;

            foreach (IPAddress ip in ips)
            {
                result[i] = ip.ToString(); i++;
            }
            return result;
        }
        public static string GetLocalIP()
        {
            foreach (string ip in GetMyIPs())
                if (ip.Split('.')[0] == "192" && ip.Split('.')[1] == "168")          
                    return ip;               
            
            return "127.0.0.1";
        }
        public static string GetIP()
        {
            return new System.Net.WebClient().DownloadString("https://api.ipify.org");
        }
    }
    /*class Chat
    {
        internal void Disconnect()
        {
            if (socket.Connected)
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Disconnect(true);
                if (socket.Connected)
                    MessageBox.Show("We're still connnected");
                else
                    MessageBox.Show("We're disconnected");
            }
        }
    }
    

    class Listener : Form
    {
        
        internal ListBox outBox { get; set; }
        internal ListBox ConnectionBox { get; set; }
        public Listener() { }

        internal int ServerPort { get; set; }
        internal Socket listener;

        internal void Disconnect()
        {
            listener.Shutdown(SocketShutdown.Both);

            listener.Disconnect(true);
            if (listener.Connected)
                MessageBox.Show("We're still connnected");
            else
                MessageBox.Show("We're disconnected");

        }
    }*/
}
