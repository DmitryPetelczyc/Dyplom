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
    delegate void MessageHandler(string message);
    delegate void ConnectionHandler(bool con, bool first = false);
    delegate void incoming_pocketsHandler();
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

    internal class Motoko
    {
        internal IPAddress CallBackIPAddress;
        internal int CallBackPort;
        //Адрес получателя
        internal IPEndPoint remoteEP;
        internal string global_name;
        internal string user_public_key;

        //Заморозка потока при подключении, отправки, получении данных
        private static readonly ManualResetEvent sendDone =
                new ManualResetEvent(false);
        private static readonly ManualResetEvent connectDone =
                new ManualResetEvent(false);
        private static readonly ManualResetEvent receiveDone = 
                new ManualResetEvent(false);

        internal Socket sock;
        internal Socket listener;

        MessageHandler _del;
        ConnectionHandler _del_con;
        //==================================================================================
        public Motoko(string CallBackIPAddress, int CallBackPort)
        {
            this.CallBackIPAddress = IPAddress.Parse(CallBackIPAddress);
            this.CallBackPort = CallBackPort;
        }
        public void GetConnectionHandler(ConnectionHandler del)
        {
            _del_con = del;
        }
        public void GetMessageHandler(MessageHandler del)
        {
            _del = del;
        }
        public class StateObject
        {
            public Socket workSocket = null;
            public const int BufferSize = 256;
            public byte[] buffer = new byte[BufferSize];
            public StringBuilder sb = new StringBuilder();
        }
        //==================================================================================
        internal void first_connection()
        {
            sock = new Socket(AddressFamily.InterNetwork,
                              SocketType.Stream,
                              ProtocolType.Tcp);

            sock.BeginConnect(remoteEP,
                new AsyncCallback(first_ConnectCallback), sock);

            connectDone.WaitOne();
        }
        private void first_ConnectCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                sock = (Socket)ar.AsyncState;

                // Complete the connection.  
                sock.EndConnect(ar);

                // Signal that the connection has been made.  
                connectDone.Set();

                _del_con.Invoke(true, true);

            }
            catch (Exception e)
            {
                _del_con.Invoke(false, true);
                //Делегировать событие о неудачном подключении 
                connectDone.Set();
            }
        }
        internal void connection()
        {
            sock = new Socket(AddressFamily.InterNetwork,
                              SocketType.Stream,
                              ProtocolType.Tcp);

            sock.BeginConnect(remoteEP,
                new AsyncCallback(ConnectCallback), sock);

            connectDone.WaitOne();
        }
        private void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                sock = (Socket)ar.AsyncState;

                // Complete the connection.  
                sock.EndConnect(ar);

                // Signal that the connection has been made.  
                connectDone.Set();

                _del_con.Invoke(true);

                crypted_send_message(CallBackIPAddress + ":" + CallBackPort, "0");

                Main._connectionDone.Set();
            }
            catch (Exception e)
            {
                _del_con.Invoke(false);

                connectDone.Set();
            }
        }
        //==================================================================================
        internal void crypted_send_message(string data, string code)
        {
            UnicodeEncoding ByteConverter = new UnicodeEncoding();

            byte[] publicKey = Convert.FromBase64String(user_public_key);  //File.ReadAllBytes(@"./public.txt");//Convert.FromBase64String(user_public_key);

            //Буфер для данных для шифровки
            byte[] byteData = new byte[256];
            // ` - системный разделитель
            //Конвертация кода и сообщения в байты            

            //MessageBox.Show(code + "`" + data + "\n\n" + Convert.ToBase64String(publicKey));

            if (code == "0")
                byteData = rsa.RSAEncrypt(ByteConverter.GetBytes(code + "`" + data + "`" + Main.my_global_name + "`#"), publicKey, false);
            else if (code == "2")
                byteData = rsa.RSAEncrypt(ByteConverter.GetBytes(code + "`" + data), publicKey, false);
            else if (code == "21")
                byteData = rsa.RSAEncrypt(ByteConverter.GetBytes(code + "`" + data), publicKey, false);
            else if (code == "3")
                byteData = rsa.RSAEncrypt(ByteConverter.GetBytes(data), publicKey, false);
            else if (code == "4")
                byteData = rsa.RSAEncrypt(ByteConverter.GetBytes(code + "`" + data), publicKey, false);
            else if (code == "5")
                byteData = rsa.RSAEncrypt(ByteConverter.GetBytes(code + "`" + data), publicKey, false);
            else if (code == "6")
                byteData = rsa.RSAEncrypt(ByteConverter.GetBytes(code + "`" + data), publicKey, false);

            //Отправка данных          
            sock.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(sendCallback), sock);
        }
        static void sendCallback(IAsyncResult ar)
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
                Main.file_sending.Set();
            }
            catch (Exception ex)
            {
                MessageBox.Show("" + ex.Message);
            }
        }
        //==================================================================================
        incoming_pocketsHandler inc_pock;
        public void GetIncomingPocketsHandler(incoming_pocketsHandler del)
        {
            inc_pock = del;
        }
        internal void StartListening()
        {
            try
            {
                IPEndPoint localEP = new IPEndPoint(IPAddress.Any, CallBackPort);
                listener = new Socket(AddressFamily.InterNetwork,
                                               SocketType.Stream,
                                               ProtocolType.Tcp);

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
        public void AcceptCallback(IAsyncResult ar)
        {
            // Get the socket that handles the client request.  
            Socket listener = (Socket)ar.AsyncState;
            try
            {
                Socket handler = listener.EndAccept(ar);

                // Signal the main thread to continue.  
                receiveDone.Set();

                // Create the state object.  
                StateObject state = new StateObject
                {
                    workSocket = handler
                };
                handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReadCallback), state);

            }
            catch (ObjectDisposedException) {; } //rise when port is changed
            catch (Exception ex) { MessageBox.Show(ex.ToString()); }
        }
        public void ReadCallback(IAsyncResult ar)
        {
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;
            byte[] privateKey = Convert.FromBase64String(Main.private_key);

            inc_pock?.Invoke();

            int read = 0;

            // Read data from the client socket.  
            try 
            { 
                read = handler.EndReceive(ar);             
            }
            catch (SocketException ex)
            {
                //host disconnect
            }

            // Data was read from the client socket.  
            if (read > 0)
            {
                UnicodeEncoding ByteConverter = new UnicodeEncoding();
                //Decrypting 
                string temp = ByteConverter.GetString(rsa.RSADecrypt(state.buffer, privateKey, false));
                //string temp = Convert.ToBase64String(state.buffer);


                if (temp[0] != 0)
                    state.sb.Append(temp, 0, temp.Length);

                if (state.sb.ToString()[state.sb.ToString().Length - 2] == '`' &&
                    state.sb.ToString()[state.sb.ToString().Length - 1] == '#')
                {
                    _del?.Invoke(state.sb.ToString());
                    state.sb.Clear();
                }

                handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReadCallback), state);

                receiveDone.WaitOne();

                state.sb.Clear();
            }
        }
        //==================================================================================
    }
}
