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
    class SQLite
    {
        public SQLiteConnection connection = new SQLiteConnection();
        public SQLiteDataAdapter adapter = new SQLiteDataAdapter();
        public void Connect(string db_name)
        {
            connection.ConnectionString = "Data Source = " + db_name + "; Version = 3;";
            try
            {
                connection.Open();
                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " || " + ex);
                //Console.WriteLine(ex.Message + " || " + ex);
            }

        }
        public void ExecuteRequest(string cmdText)
        {
            try
            {
                if (connection.ConnectionString == null)
                    Connect(@"./DataBases/DataBase.db3");

                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(connection))
                {
                    command.CommandText = @"" + cmdText;
                    command.CommandType = CommandType.Text;
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " || " + ex);
                connection.Close();
                //Console.WriteLine(ex.Message + " || " + ex);
            }
        }
        public void Select(string cmdText, DataGridView Grid)
        {
            try
            {
                if (connection.ConnectionString == null)
                    Connect(@"./DataBases/DataBase.db3");

                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(connection))
                {
                    command.CommandText = @"" + cmdText;
                    command.CommandType = CommandType.Text;
                    command.ExecuteNonQuery();
                    adapter.SelectCommand = command;
                    System.Data.DataSet dataSet = new DataSet();
                    adapter.Fill(dataSet);
                    Grid.DataSource = dataSet.Tables[0];
                    adapter.Update(dataSet);
                }
                connection.Close();
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
                if (connection.ConnectionString == null)
                    Connect(@"./DataBases/DataBase.db3");

                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(connection))
                {
                    command.CommandText = @"" + cmdText;
                    command.CommandType = CommandType.Text;
                    command.ExecuteNonQuery();
                    adapter.SelectCommand = command;
                    System.Data.DataSet dataSet = new DataSet();
                    adapter.Fill(dataSet);
                    Box.DataSource = dataSet.Tables[0];
                    adapter.Update(dataSet);
                }
                connection.Close();
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
                if (connection.ConnectionString == null)
                    Connect(@"./DataBases/DataBase.db3");

                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(connection))
                {
                    command.CommandText = @"" + cmdText;
                    command.CommandType = CommandType.Text;
                    command.ExecuteNonQuery();
                    adapter.SelectCommand = command;
                    System.Data.DataSet dataSet = new DataSet();
                    adapter.Fill(dataSet);
                    adapter.Update(dataSet);
                    return dataSet;
                }
                connection.Close();
                //DataSet teams = SQL_DATASET("SELECT [column] FROM [table]");
                //teams.Tables[0].Rows[0].ItemArray[0].ToString();
            }
            catch (Exception ex)
            {
                connection.Close();
                MessageBox.Show(ex.Message + " || " + ex);
                //Console.WriteLine(ex.Message + " || " + ex);
                return null;
            }
        }
        public void Insert_BLOB(string cmdText)
        {
            FileStream fs = null;
            string path = @"./components/treeView/tree.xml";
            
            fs = new FileStream(path, FileMode.Open);  // открываем файл
            byte[] fileBuffer = new byte[fs.Length];
            fs.Read(fileBuffer, 0, (int)fs.Length);    // читаем в бинарный буфер
            fs.Close();

            if (connection.ConnectionString == null)
                Connect(@"./DataBases/DataBase.db3");
            
            connection.Open();
            using (SQLiteCommand command = new SQLiteCommand(connection))
            {
                command.CommandText = @"" + cmdText;//"INSERT INTO treeViews VALUES (NULL, 'test', @file)";         // команда добавления файла в бинарном виде//command.CommandText = @"" + cmdText;
                command.CommandType = CommandType.Text;
                command.Parameters.Add("@file", System.Data.DbType.Binary).Value = fileBuffer; // записываем бинарный буфер в значение параметра
                command.ExecuteNonQuery();
            }
            connection.Close();
        }
        public bool Check_table_exist(string Table) 
        {
            try
            {
                if (connection.ConnectionString == null)
                    Connect(@"./DataBases/DataBase.db3");

                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(connection))
                {
                    command.CommandText = @"SELECT * FROM '" + Table + "' LIMIT 1; ";
                    command.CommandType = CommandType.Text;
                    command.ExecuteNonQuery();
                }
                connection.Close();
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
        SQLite sqlite = new SQLite();
        public DataBase()
        { 
            
        }
        internal void create_new_data_base()
        {
            string path = @"./DataBases/DataBase.db3";
            if (!File.Exists(path))
            {
                SQLiteConnection.CreateFile(path);

                sqlite.Connect(path);

                sqlite.ExecuteRequest("CREATE TABLE user_info (" +
                                      "name VARCHAR(30), " +
                                      "public_key TEXT, " +
                                      "private_key TEXT)");

                sqlite.ExecuteRequest("CREATE TABLE treeViews (" +
                                      "id INTEGER PRIMARY KEY AUTOINCREMENT, " +
                                      "name VARCHAR(30), " +
                                      "tree BLOB)");
                
                RSACryptoServiceProvider rSA = new RSACryptoServiceProvider();
                var privateKey = rSA.ToXmlString(true);
                var publicKey = rSA.ToXmlString(false);

                string EncodedStringPrivate = Convert.ToBase64String(Encoding.UTF8.GetBytes(privateKey));
                string EncodedStringPublic = Convert.ToBase64String(Encoding.UTF8.GetBytes(publicKey));

                sqlite.ExecuteRequest("INSERT INTO user_info VALUES(NULL, '"+EncodedStringPublic+"', '"+EncodedStringPrivate+"')");

                DataSet ds = sqlite.Select_DataSet("SELECT * FROM user_info");
                //MessageBox.Show(ds.Tables[0].Rows[0].ItemArray[2].ToString()); //show private key
                sqlite.connection.Close();
            }
            else
            {
                MessageBox.Show("Базы с таким название уже существует");
                File.Delete(@"./DataBases/DataBase.db3");
                return;
            }
        }
    }
}
