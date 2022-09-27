using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Security.Cryptography;

namespace ghost
{
    internal class SQLite
    {
        public SQLiteConnection connection = new SQLiteConnection();
        public SQLiteDataAdapter adapter = new SQLiteDataAdapter();
        public void Connect()
        {
            connection = new SQLiteConnection();
            string db_name = @"./DataBases/DataBase.db3";
            connection.ConnectionString = "Data Source = " + db_name + "; Version = 3;";

            try
            {
                connection.Open();
                connection.Close();
            }
            catch (Exception ex)
            {
                connection.Close();
                MessageBox.Show(ex.Message + " || " + ex);
                //Console.WriteLine(ex.Message + " || " + ex);
            }
        }
        public void ExecuteRequest(string cmdText)
        {
            try
            {
                Connect();
                using (SQLiteCommand command = new SQLiteCommand(connection))
                {
                    command.CommandText = @"" + cmdText;
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                connection.Close();
                MessageBox.Show(ex.Message + " || " + ex);
                //Console.WriteLine(ex.Message + " || " + ex);
            }
        }
        public void Select(string cmdText, DataGridView Grid)
        {
            try
            {
                Connect();
                using (SQLiteCommand command = new SQLiteCommand(connection))
                {
                    command.CommandText = @"" + cmdText;
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                    adapter.SelectCommand = command;
                    DataSet dataSet = new DataSet();
                    adapter.Fill(dataSet);
                    Grid.DataSource = dataSet.Tables[0];
                    adapter.Update(dataSet);
                }
            }
            catch (Exception ex)
            {
                connection.Close();
                MessageBox.Show(ex.Message + " || " + ex);
                //Console.WriteLine(ex.Message + " || " + ex);
            }
        }
        public void Select(string cmdText, ComboBox Box)
        {
            try
            {
                Connect();
                using (SQLiteCommand command = new SQLiteCommand(connection))
                {
                    command.CommandText = @"" + cmdText;
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                    adapter.SelectCommand = command;
                    System.Data.DataSet dataSet = new DataSet();
                    adapter.Fill(dataSet);
                    Box.DataSource = dataSet.Tables[0];
                    adapter.Update(dataSet);
                }
            }
            catch (Exception ex)
            {
                connection.Close();
                MessageBox.Show(ex.Message + " || " + ex);
                //Console.WriteLine(ex.Message + " || " + ex);
            }
        }
        public DataSet Select_DataSet(string cmdText)
        {
            try
            {
                Connect();
                using (SQLiteCommand command = new SQLiteCommand(connection))
                {
                    command.CommandText = @"" + cmdText;
                    command.CommandType = CommandType.Text;
                    
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                    
                    adapter.SelectCommand = command;
                    DataSet dataSet = new DataSet();
                    adapter.Fill(dataSet);
                    adapter.Update(dataSet);
                    
                    return dataSet;
                }
            }
            catch (Exception ex)
            {
                connection.Close();
                MessageBox.Show(ex.Message + " || " + ex);
                //Console.WriteLine(ex.Message + " || " + ex);
                return null;
            }
        }
        public void Insert_BLOB(string cmdText, string book_name = @"tree.xml")
        {
            string path = @"./components/treeView/" + book_name;
            FileStream fs = new FileStream(path, FileMode.Open);  // открываем файл
            byte[] fileBuffer = new byte[fs.Length];
            fs.Read(fileBuffer, 0, (int)fs.Length);    // читаем в бинарный буфер
            fs.Close();

            Connect();
            
            using (SQLiteCommand command = new SQLiteCommand(connection))
            {
                command.CommandText = @"" + cmdText;//"INSERT INTO treeViews VALUES (NULL, 'test', @file)";         // команда добавления файла в бинарном виде//command.CommandText = @"" + cmdText;
                command.CommandType = CommandType.Text;
                command.Parameters.Add("@file", System.Data.DbType.Binary).Value = fileBuffer; // записываем бинарный буфер в значение параметра
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
        }
        public bool Check_table_exist(string Table) 
        {
            try
            {
                Connect();
                using (SQLiteCommand command = new SQLiteCommand(connection))
                {
                    command.CommandText = @"SELECT * FROM '" + Table + "' LIMIT 1; ";
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
                return true;
            }
            catch
            {
                connection.Close();
                return false;
            }
        }
    }

    class DataBase
    {
        private readonly SQLite sqlite = new SQLite();
        public DataBase()
        { 
            
        }
        internal void create_new_data_base()
        {
            string path = @"./DataBases/DataBase.db3";
            if (!File.Exists(path))
            {
                SQLiteConnection.CreateFile(path);

                sqlite.Connect();

                sqlite.ExecuteRequest("CREATE TABLE user_info (" +
                                      "name VARCHAR(30), " +
                                      "public_key TEXT, " +
                                      "private_key TEXT)");

                sqlite.ExecuteRequest("CREATE TABLE treeViews (" +
                                      "id INTEGER PRIMARY KEY AUTOINCREMENT, " +
                                      "name VARCHAR(30), " +
                                      "tree BLOB)");
                sqlite.Insert_BLOB("INSERT INTO treeViews VALUES(NULL, 'book', @file)", 
                                                                     @"empty_tree.xml");

                sqlite.ExecuteRequest("CREATE TABLE autosend_mesg (" +
                                      "id INTEGER PRIMARY KEY AUTOINCREMENT, " +
                                      "address VARCHAR(30), " +
                                      "name VARCHAR(30), " +
                                      "message text, " + 
                                      "global_name VARCHAR(65))");

                sqlite.ExecuteRequest("CREATE TABLE Keys (" +
                                      "id INTEGER PRIMARY KEY AUTOINCREMENT, " +
                                      "global_name VARCHAR(65), " +
                                      "open_key TEXT)");

                sqlite.ExecuteRequest("CREATE TABLE names (" +
                                      "id INTEGER PRIMARY KEY AUTOINCREMENT, " +
                                      "global_name VARCHAR(65), " +
                                      "name TEXT)");

                RSACryptoServiceProvider rSA = new RSACryptoServiceProvider(2048);
                var privateKey = rSA.ExportCspBlob(true);
                var publicKey = rSA.ExportCspBlob(false);

                string EncodedStringPrivate = Convert.ToBase64String(privateKey);
                string EncodedStringPublic = Convert.ToBase64String(publicKey);

                StringBuilder builder = new StringBuilder();
                using (SHA256 sha256Hash = SHA256.Create())
                {
                    // ComputeHash - returns byte array  
                    byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(EncodedStringPublic));

                    // Convert byte array to a string   
                    for (int i = 0; i < bytes.Length; i++)
                    {
                        builder.Append(bytes[i].ToString("x2"));
                    }
                }

                sqlite.ExecuteRequest("INSERT INTO user_info VALUES('" + builder.ToString() + "', '" + EncodedStringPublic + "', '" + EncodedStringPrivate + "')");
            }
            else
            {
                MessageBox.Show("База с таким название уже существует");
                //File.Delete(@"./DataBases/DataBase.db3");
                return;
            }
        }
    }
}
