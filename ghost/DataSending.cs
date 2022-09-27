using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ghost
{
    public partial class Main : Form
    {
        private void send_to_user(KeyEventArgs e)
        {
            if (send_message_text_box.Text.Trim() != "")
            {
                if (e.KeyCode == Keys.Enter && !e.Shift)
                {
                    string sign_hash = "";
                    string text;
                    string message = send_message_text_box.Text.Trim();
                    string mesg = message;
                    send_message_text_box.Text = "";
                    object[] row = new object[5];

                    UnicodeEncoding ByteConverter = new UnicodeEncoding();

                    if (isSign)
                    {
                        byte[] plainText = ByteConverter.GetBytes(message);

                        var rsaWrite = new RSACryptoServiceProvider();
                        var privateParams = Convert.FromBase64String(private_key); //rsaWrite.ExportCspBlob(true);
                        rsaWrite.ImportCspBlob(privateParams);

                        byte[] signature =
                            rsaWrite.SignData(plainText, new SHA256CryptoServiceProvider());

                        sign_hash = Convert.ToBase64String(signature);

                        message = message + "`#" + sign_hash + "#";

                    }

                    motoko.connection();

                    if (current_dialog != null && current_dialog != my_global_name)
                        if (isSign)
                            sqlite.ExecuteRequest("INSERT INTO '" + current_dialog +
                                "' VALUES (NULL, NULL, '" + DateTime.Now + "','" + message.Split('`') + "', '" + sign_hash + "')");
                        else
                            sqlite.ExecuteRequest("INSERT INTO '" + current_dialog +
                        "' VALUES (NULL, NULL, '" + DateTime.Now + "','" + message + "', NULL)");


                    decimal d = Convert.ToDecimal(message.Length) / 95;
                    if (message != "")
                    {
                        for (int i = 0; i < Math.Ceiling(d); i++)
                        {
                            try
                            {
                                if (message.Length > 94)
                                {
                                    text = message.Substring(0, 94);
                                    message = message.Substring(94);
                                }
                                else
                                {
                                    text = message;
                                }

                                if (i == 0)
                                {
                                    if (isSign)
                                    {
                                        motoko.crypted_send_message(
                                            motoko.CallBackIPAddress + ":" +
                                            motoko.CallBackPort + "`" +
                                            text, "21");
                                    }
                                    else
                                    {
                                        motoko.crypted_send_message(
                                            motoko.CallBackIPAddress + ":" +
                                            motoko.CallBackPort + "`" +
                                            text, "2");
                                    }
                                }
                                else
                                {
                                    motoko.crypted_send_message(text, "3");
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.ToString());
                                send_message_text_box.Enabled = false;
                                send_message_text_box.Text = "Пользователь вне сети...";
                                return;
                            }
                        }

                        motoko.crypted_send_message("`" + my_global_name + "`#", "3");

                        DataSet ds = sqlite.Select_DataSet("SELECT id FROM '" + current_dialog + "' ORDER BY id DESC LIMIT 1");

                        string date = DateTime.Now.ToString().Split(' ')[1];

                        if (ds.Tables[0].Rows.Count > 0)
                            row[0] = ds.Tables[0].Rows[0].ItemArray[0].ToString();
                        else
                            row[0] = "1";

                        row[2] = date.Split(':')[0] + ":" + date.Split(':')[1];
                        row[3] = mesg;

                        if (isSign)
                        {
                            row[4] = "True";
                        }


                        if (current_dialog != my_global_name)
                        {
                            message_view_dgv.Rows.Insert(message_view_dgv.Rows.Count, row);

                            message_view_dgv.Rows[message_view_dgv.Rows.Count - 1].Cells[3].Style.Alignment =
                                                                DataGridViewContentAlignment.MiddleRight;

                            if (isSign)
                            {
                                message_view_dgv.Rows[message_view_dgv.Rows.Count - 1].Cells[3].Style.Font =
                                    new Font("Aire Exterior", 12);

                                message_view_dgv.Rows[message_view_dgv.Rows.Count - 1].Cells[3].Style.ForeColor =
                                    Color.FromArgb(120, 198, 200);
                            }
                            message_view_dgv.FirstDisplayedScrollingRowIndex = message_view_dgv.RowCount - 1;
                        }
                    }
                }
            }
        }
        private bool sending_to_group(string message, string global_name, string group)
        {
            string text;
            decimal d = Convert.ToDecimal(message.Length) / 121;

            for (int i = 0; i < Math.Ceiling(d); i++)
            {
                try
                {
                    if (message.Length > 121)
                    {
                        text = message.Substring(0, 120);
                        message = message.Substring(120);
                    }
                    else
                    {
                        text = message;
                    }

                    if (i == 0)
                    {
                        motoko.crypted_send_message(group + "`" + 
                            motoko.CallBackIPAddress + ":" + motoko.CallBackPort + "`", "4");
                    }

                    motoko.crypted_send_message(text, "3");
                }
                catch
                {
                    return false;
                }
            }

            motoko.crypted_send_message("`" + global_name + "`#", "3");
            return true;
        }
        private void check_in(TreeNode node, ref TreeNode[] massive, int ImageIndex)
        {
            foreach (TreeNode _node in node.Nodes)
            {
                if (_node.ImageIndex == ImageIndex)
                {
                    Array.Resize(ref massive, massive.Length + 1);
                    massive[massive.Length - 1] = _node;
                }

                if (_node.Nodes.Count > 0)
                {
                    check_in(_node, ref massive, ImageIndex);
                }
            }
        }
        private string[] get_group_tag_by_adrress(TreeNode node)
        {
            string[] ids = new string[0];
            int j = node.Level;
            for (int i = 0; i < j; i++)
            {
                if (node.ImageIndex == 4)
                {
                    Array.Resize(ref ids, ids.Length + 1);
                    ids[ids.Length - 1] = node.Tag.ToString();
                }
                node = node.Parent;
            }
            return ids;
        }
        private void send_to_group(KeyEventArgs e)
        {
            TreeNode[] treeNodes = new TreeNode[0];
            
            if (send_message_text_box.Text.Trim() != "")
            {
                if (e.KeyCode == Keys.Enter && !e.Shift)
                {
                    TreeNode node = get_parent_node(current_dialog);

                    foreach (TreeNode _node in node.Nodes)
                    {
                        if (_node.Nodes.Count > 0)
                        {
                            check_in(_node, ref treeNodes, 2);
                        }
                    }

                    UnicodeEncoding ByteConverter = new UnicodeEncoding();
                    string message = send_message_text_box.Text.Trim();
                    send_message_text_box.Text = "";
                    object[] row = new object[5];

                    foreach (TreeNode _node in treeNodes)
                    {
                        string ip = _node.Tag.ToString().Split(':')[0];
                        int port = Convert.ToInt32(_node.Tag.ToString().Split(':')[1]);

                        motoko.remoteEP = new IPEndPoint(IPAddress.Parse(ip), port);
                        motoko.user_public_key = get_public_key_by_blobal_name(_node.Parent.Tag.ToString());

                        send_message_text_box.Text = "Подключение...";
                        send_message_text_box.Enabled = false;

                        motoko.connection();

                        Thread.Sleep(200);

                        send_message_text_box.Text = "Отправка...";
                        send_message_text_box.Enabled = false;

                        if (sending_to_group(message, my_global_name, current_dialog))
                        {
                            //there u can handle every sended message
                        }

                        send_message_text_box.Text = "";
                        send_message_text_box.Enabled = true;
                    }

                    DataSet ds = new DataSet();

                    sqlite.ExecuteRequest("INSERT INTO '" + current_dialog +
                        "' VALUES (NULL, NULL, '" + DateTime.Now + "','" + message + "', NULL)");

                    ds = sqlite.Select_DataSet("SELECT id FROM '" + current_dialog + "' ORDER BY id DESC LIMIT 1");

                    string date = DateTime.Now.ToString().Split(' ')[1];

                    if (ds.Tables.Count > 0)
                        row[0] = ds.Tables[0].Rows[0].ItemArray[0].ToString();
                    else
                        row[0] = "1";

                    row[2] = date.Split(':')[0] + ":" + date.Split(':')[1];
                    row[3] = message;
                    row[4] = "";

                    if (current_dialog != my_global_name)
                    {
                        message_view_dgv.Rows.Insert(message_view_dgv.Rows.Count, row);

                        message_view_dgv.Rows[message_view_dgv.Rows.Count - 1].Cells[3].Style.Alignment =
                                                            DataGridViewContentAlignment.MiddleRight;
                    }
                }
            }
        }
    }
}
