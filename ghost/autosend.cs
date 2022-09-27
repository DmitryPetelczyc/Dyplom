using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ghost
{
    public partial class Main : Form
    {
        //===========================[autosend methods]=====================================
        int row_index = 0;
        DataGridViewRow row;
        Motoko autosend = new Motoko(local_ip, 3000);
        Thread autosend_thread;

         private void autosend_textbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (treeView1.SelectedNode != null && treeView1.SelectedNode.ImageIndex == 2)
            {
                if (autosend_textbox.Text.Trim() != "")
                {
                    if (e.KeyCode == Keys.Enter && !e.Shift)
                    {
                        string global_name = treeView1.SelectedNode.Parent.Tag.ToString();
                        string address = treeView1.SelectedNode.Tag.ToString();
                        string message = autosend_textbox.Text.Trim();
                        string name = treeView1.SelectedNode.Text.ToString();

                        sqlite.ExecuteRequest("INSERT INTO autosend_mesg VALUES " +
                            "(NULL, '" + address + "', '" + name + "', " +
                            "'" + message + "', '" + global_name + "')");
                        autosend_textbox.Text = "";
                        fill_autosend_dgv();
                    }
                }
            }
            else
            {
                MessageBox.Show("choose node");
            }
        }
        private void autosend_result(bool con, bool first)
        {
            if (con)
            {
                string text;
                string message = row.Cells[3].Value.ToString().Trim();
                decimal d = Convert.ToDecimal(message.Length) / 100;
                if (message != "")
                {
                    for (int i = 0; i < Math.Ceiling(d); i++)
                    {
                        try
                        {
                            if (message.Length > 99)
                            {
                                text = message.Substring(0, 99);
                                message = message.Substring(99);
                            }
                            else
                            {
                                text = message;
                            }

                            if (i == 0)
                            {
                                autosend.crypted_send_message(
                                    autosend.CallBackIPAddress +
                                    ":" + autosend.CallBackPort +
                                    "`" + text, "2");
                            }
                            else
                            {
                                autosend.crypted_send_message(text, "3");
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.ToString());
                        }
                    }

                    autosend.crypted_send_message("`" + my_global_name + "`#", "3");

                    sqlite.ExecuteRequest("DELETE FROM autosend_mesg WHERE id LIKE " + row.Cells[0].Value + "");
                    sqlite.ExecuteRequest("INSERT INTO `" + row.Cells[4].Value + "` " +
                        "VALUES (NULL, NULL, '" + DateTime.Now + "', '" + row.Cells[3].Value.ToString() + "', NULL)");


                    if (InvokeRequired)
                        Invoke(new Action(() => fill_autosend_dgv()));
                    else
                        fill_autosend_dgv();

                    if (autosend_dgv.Rows.Count > row_index)
                    {
                        send_autosend_messages();
                    }
                    else
                    {
                        row_index = 0;
                    }
                }
            }
            else
            {
                if (autosend_dgv.Rows.Count > row_index)
                {
                    row_index++;
                    send_autosend_messages();
                }
                else
                {
                    row_index = 0;
                }
            }
        }
        private void send_autosend_messages()
        {
            if (row_index < autosend_dgv.Rows.Count)
            {

                IPAddress address;
                int id;
                try
                {
                    row = autosend_dgv.Rows[row_index];

                    id = Convert.ToInt32(row.Cells[0].Value);
                    address = IPAddress.Parse(row.Cells[1].Value.ToString().Split(':')[0]);
                    int port = Convert.ToInt32(row.Cells[1].Value.ToString().Split(':')[1]);

                    autosend.remoteEP = new IPEndPoint(address, port);
                    autosend.user_public_key = get_public_key_by_blobal_name(row.Cells[4].Value.ToString());
                    autosend.global_name = row.Cells[4].Value.ToString();

                    if (InvokeRequired)
                        Invoke(new Action(() => autosend.connection()));
                    else
                        autosend.connection();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }
        private void autosend_timer_Tick(object sender, EventArgs e)
        {
            if (Convert.ToInt32(timer_label.Text) == 00)
            {
                row_index = 0;
                autosend_thread = new Thread(new ThreadStart(send_autosend_messages));
                autosend_thread.Start();

                timer_label.Text = "15";
            }
            else
            {
                if (Convert.ToInt32(timer_label.Text) < 11)
                    timer_label.Text = "0" + (Convert.ToInt32(timer_label.Text) - 1).ToString();
                else
                    timer_label.Text = (Convert.ToInt32(timer_label.Text) - 1).ToString();
            }
        }

        private void fill_autosend_dgv()
        {
            sqlite.Select("SELECT * FROM autosend_mesg", autosend_dgv);
            autosend_dgv.Columns[0].Visible = false;
            autosend_dgv.Columns[1].Visible = false;
            autosend_dgv.Columns[4].Visible = false;
        }
        private void delete_row_from_autosend_dgv(object sender, EventArgs e)
        {
            if (autosend_dgv.SelectedRows != null)
            {
                int id = Convert.ToInt32(autosend_dgv.SelectedRows[0].Cells[0].Value);
                sqlite.ExecuteRequest("DELETE FROM autosend_mesg WHERE id LIKE " + id + "");
                fill_autosend_dgv();
            }
        }
        private void autosend_dgv_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                ContextMenu m = new ContextMenu();

                int currentMouseOverRow = autosend_dgv.HitTest(e.X, e.Y).RowIndex;

                if (currentMouseOverRow >= 0)
                {
                    m.MenuItems.Add(new MenuItem("Удалить"));
                    m.MenuItems[0].Click += new EventHandler(delete_row_from_autosend_dgv);
                    autosend_dgv.Rows[currentMouseOverRow].Selected = true;
                }

                m.Show(autosend_dgv, new Point(e.X, e.Y));
            }
        }
    }
}
