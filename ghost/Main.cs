using System;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace ghost
{
    public partial class Main : Form
    {
        SQLite sqlite = new SQLite();

        Thread receive;
        Thread conWindowAnimation;
        Thread conn;

        internal static readonly ManualResetEvent _connectionDone =
            new ManualResetEvent(false);
        internal static readonly ManualResetEvent _first_connectionDone =
            new ManualResetEvent(false);
        internal static readonly ManualResetEvent file_sending =
            new ManualResetEvent(false);

        internal static string local_ip = "" + Dns.GetHostByName(Dns.GetHostName()).AddressList[Dns.GetHostByName(Dns.GetHostName()).AddressList.Length - 1];

        Motoko motoko = new Motoko(local_ip, 1000);
        //Motoko_autosend motoko_as = new Motoko_autosend(local_ip, 3000, "127.0.0.1", 0);

        internal static Color CustomForeColor = Color.White;//Color.FromArgb(120, 198, 200);
        internal static Color FormBackColor = Color.FromArgb(29, 29, 36);
        internal static Color CustomBackColor = Color.FromArgb(35, 35, 45);

        Image[] imglist = new Image[6];

        internal static string my_global_name;
        internal static string public_key;
        internal static string private_key;

        string current_connection;
        string current_dialog;
        int current_slider = 0;
        bool isSign = false;

        bool group = false; //Если пишем в группу будет тру если юзверю то фолс

        TreeNode renaming_node;
        private int win_anim_offset;
        private const int cGrip = 16;      // Grip size

        //===============================[Language Setings]=================================
        //set massive for words (current language)
        public static string[] Lang = new string[100];
        private void LangInit()
        {
            //get lang file
            using (StreamReader sr =
                new StreamReader(@"./lang/lang.lang", System.Text.Encoding.UTF8))
            {
                string line;
                int i = 0;
                while ((line = sr.ReadLine()) != null)
                {
                    Lang[i++] = line;
                }
            }

            Node_add_Button.Text = Lang[3];
            button1.Text = Lang[18];
            button2.Text = Lang[11];

            address_textbox.Text = Lang[15];
            address_textbox.ForeColor = Color.Silver;
            global_name_textbox.Text = Lang[16];
            global_name_textbox.ForeColor = Color.Silver;
            open_key_textbox.Text = "Публичный ключ";
            open_key_textbox.ForeColor = Color.Silver;
        }
        //==================================================================================
        public Main()
        {
            InitializeComponent();
            LangInit();
            this.FormBorderStyle = FormBorderStyle.None;
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.ResizeRedraw, true);

            try
            {
                receive = new Thread(new ThreadStart(motoko.StartListening));
                motoko.GetMessageHandler(Display);
                motoko.GetConnectionHandler(connect_result);
                autosend.GetConnectionHandler(autosend_result);
                motoko.GetIncomingPocketsHandler(increment_pockets);
                //motoko_as.GetConnectionHandler(autosend_result);
                receive.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            autosend_timer.Enabled = true;
            KeyPreview = true;

            set_treeView_settings();
            database_connection();
            set_font_and_background();
            set_dgv_settings();

            DataSet ds = sqlite.Select_DataSet("SELECT * FROM user_info");
            my_global_name = ds.Tables[0].Rows[0].ItemArray[0].ToString();
            public_key = ds.Tables[0].Rows[0].ItemArray[1].ToString();
            private_key = ds.Tables[0].Rows[0].ItemArray[2].ToString();

            conWindowAnimation = new Thread(new ThreadStart(conWinAnimation));

            foreach (TreeNode node in treeView1.Nodes)
                Set_SelectedImageIndex_in_treeView_like_IamgeIndex(node);

            textBox1.Text = my_global_name;
            textBox2.Text = motoko.CallBackIPAddress.ToString() + ":" + motoko.CallBackPort.ToString();
            textBox3.Text = public_key;

            file_send_panel.BackgroundImage = Image.FromFile("./images/document.png");
            file_send_panel.BackgroundImageLayout = ImageLayout.Stretch;
        }
        //=========================[MainForm move and resize]===============================
        //Не выпускай кракена
        private void Main_Resize(object sender, EventArgs e)
        {
            int top_margin = 45;
            int component_margin = 15;
            int bottom_margin = 35;
            int summary_margin = component_margin;

            toolbox_panel.Size = new Size(50, Height - bottom_margin - component_margin);
            toolbox_panel.Location = new System.Drawing.Point(component_margin, top_margin);

            summary_margin += toolbox_panel.Size.Width + component_margin;

            treeView1.Size = new Size(Convert.ToInt32(Width * 0.25), Height - bottom_margin - component_margin - 10 - 26);
            treeView1.Location = new System.Drawing.Point(summary_margin, top_margin + 10 + 26);

            panel1.Size = new Size(Convert.ToInt32(Width * 0.25), Height - bottom_margin - component_margin - 60);
            panel1.Location = new System.Drawing.Point(summary_margin, top_margin + 26);

            panel2.Size = new Size(treeView1.Width, 24);
            panel2.Location = new Point(summary_margin, top_margin);
            panel2.BackColor = CustomBackColor;

            add_user_panel.BackColor = CustomBackColor;
            add_user_panel.BackgroundImage = Image.FromFile(@"./Images/ico/treeView/user.png");
            add_folder_panel.BackColor = CustomBackColor;
            add_folder_panel.BackgroundImage = Image.FromFile(@"./Images/ico/treeView/folder.png");
            add_group_panel.BackColor = CustomBackColor;
            add_group_panel.BackgroundImage = Image.FromFile(@"./Images/ico/treeView/group.png");
            help_panel.BackColor = CustomBackColor;
            help_panel.BackgroundImage = Image.FromFile(@"./Images/about.png");

            summary_margin += panel1.Width + component_margin;

            panel_home.Location = new System.Drawing.Point(Convert.ToInt32((toolbox_panel.Width - panel_home.Width) / 2), 10);
            panel_autosend.Location = new Point(Convert.ToInt32((toolbox_panel.Width - panel_home.Width) / 2),
                Convert.ToInt32(toolbox_panel.Height - panel_settings.Height - 10) - 46);
            panel_settings.Location = new Point(Convert.ToInt32((toolbox_panel.Width - panel_home.Width) / 2),
                Convert.ToInt32(toolbox_panel.Height - panel_settings.Height - 10));

            help_panel.Location = new System.Drawing.Point(Convert.ToInt32((toolbox_panel.Width - panel_home.Width) / 2), 10 + 46);
            
            send_message_text_box.WordWrap = true;
            autosend_textbox.WordWrap = true;

            panel3.Size = new Size(Width - summary_margin - component_margin, Height + 500);
            panel3.Location = new Point(summary_margin, top_margin);

            panel4.Size = new Size(Width - summary_margin - component_margin, Height - bottom_margin - component_margin);
            panel4.Location = new Point(0, 0);

            panel5.Size = new Size(Width - summary_margin - component_margin, Height - bottom_margin - component_margin);
            panel5.Location = new System.Drawing.Point(panel4.Width, 0);

            panel6.Size = new Size(Width - summary_margin - component_margin, Height - bottom_margin - component_margin);
            panel6.Location = new System.Drawing.Point(panel4.Width * 2, 0);

            message_view_dgv.Size = new Size(Width - summary_margin - component_margin, panel4.Height - 50 - 25);
            message_view_dgv.Location = new System.Drawing.Point(0, 25);

            autosend_dgv.Size = new Size(Width - summary_margin - component_margin, panel4.Height - 50 - 25);
            autosend_dgv.Location = new System.Drawing.Point(0, 25);

            send_message_text_box.Size = new Size(Width - summary_margin - component_margin - 20 - 46, 40);
            send_message_text_box.Location = new System.Drawing.Point(10, panel4.Height - 45);

            functional_send_panel.Size = new Size(40, 40);
            functional_send_panel.Location = new Point(panel4.Width - 50, panel4.Height - 45);

            autosend_textbox.Size = new Size(Width - summary_margin - component_margin - 20, 40);
            autosend_textbox.Location = new System.Drawing.Point(10, panel4.Height - 45);

            global_name_textbox.Size = new Size(panel1.Width - 10, 27);
            address_textbox.Size = new Size(panel1.Width - 10, 27);
            open_key_textbox.Size = new Size(panel1.Width - 10, 27);

            button1.Size = new Size(Convert.ToInt32(panel1.Width / 2) - 10, 27);
            button1.Location = new Point(2, 57 + 28);

            button2.Size = new Size(Convert.ToInt32(panel1.Width / 2) - 10, 27);
            button2.Location = new Point(button1.Width + 12, 57 + 28);

            Node_add_Button.Size = new Size(Convert.ToInt32(panel1.Width / 2) - 10, 27);
            Node_add_Button.Location = new Point(button1.Width + 12, 57 + 28);

            form_close_panel.Location = new System.Drawing.Point(Width - 26 - component_margin, 5);
            fullscrin_form_panel.Location = new System.Drawing.Point(Width - 26 - component_margin - 31, 5);
            trey_form_panel.Location = new System.Drawing.Point(Width - 26 - component_margin - (31 * 2), 5);
            timer_label.Location = new System.Drawing.Point(Width - 20 - component_margin - (31 * 3), 4);

            label1.Width = panel5.Width - 15;
            label1.Location = new Point(0, 0);

            label2.Width = panel5.Width - 15;
            label2.Location = new Point(0, 0);

            panel3.HorizontalScroll.Value = 0;
        }
        private void create_table(string name)
        {
            if (!sqlite.Check_table_exist(name))
            {
                sqlite.ExecuteRequest("CREATE TABLE '" + name + "' (" +
                                                                "id INTEGER PRIMARY KEY AUTOINCREMENT, " +
                                                                "sender VARCHAR(30), " +
                                                                "date VARCHAR(30), " +
                                                                "data TEXT, " +
                                                                "sign TEXT )");
            }
        }
        //==================================================================================
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            serialize_tree_view();
            //stop receiving 
            File.Delete(@"./components/treeView/tree.xml");
            receive.Abort();
            receive.Join(500);
        }
        private void connect_result(bool con, bool first)
        {
            if (first)
            {
                if (con)
                {
                    //Create table in DataBase if not exist
                    create_table(global_name_textbox.Text);

                    Action act = new Action(() =>
                    {
                        void final()
                        {
                            current_connection = address_textbox.Text;

                            address_textbox.Text = Lang[15];
                            address_textbox.ForeColor = Color.Silver;
                            global_name_textbox.Text = Lang[16];
                            global_name_textbox.ForeColor = Color.Silver;
                            open_key_textbox.Text = "Публичный ключ";
                            open_key_textbox.ForeColor = Color.Silver;
                        }
                        void new_con()
                        {
                            TreeNode parent_node = get_parent_node(global_name_textbox.Text); //<==

                            TreeNode con_node = new TreeNode(address_textbox.Text, 2, 2)
                            {
                                Tag = address_textbox.Text
                            };

                            parent_node.Nodes.Add(con_node);
                        }
                        void new_user()
                        {
                            TreeNode parent_node = new TreeNode(get_name_by_global_name(global_name_textbox.Text), 3, 3)
                            {
                                Tag = global_name_textbox.Text
                            };
                            treeView1.Nodes.Insert(0, parent_node);
                        }

                        //Check user existing in contact book
                        if (check_on_match_in_treeView(global_name_textbox.Text))
                        {
                            //If user exist
                            //Check IP Address in exisiting user note
                            //If Address exist
                            if (check_on_match_in_treeView(address_textbox.Text))
                            {
                                //End cheking. Created nothing
                                final();
                            }
                            //If Address not exist in note
                            else
                            {
                                //Add new address in existing user note
                                new_con();
                                final();
                            }
                        }
                        //If user not exist
                        else
                        {
                            //Add new user
                            new_user();
                            //Add new address in new user
                            new_con();
                            final();
                        }

                        //==============================================================

                        send_message_text_box.Text = "";
                        send_message_text_box.Enabled = true;

                        string text;
                        string key = public_key;

                        decimal d = Convert.ToDecimal(key.Length) / 100;

                        for (int i = 0; i < Math.Ceiling(d); i++)
                        {
                            try
                            {
                                if (key.Length > 99)
                                {
                                    text = key.Substring(0, 99);
                                    key = key.Substring(99);
                                }
                                else
                                {
                                    text = key;
                                }

                                if (i == 0)
                                {
                                    motoko.crypted_send_message(text, "5");
                                }
                                else
                                {
                                    motoko.crypted_send_message(text, "3");
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.ToString());
                            }
                        }
                        motoko.crypted_send_message("`" + my_global_name + "`#", "3");

                        motoko.crypted_send_message(motoko.CallBackIPAddress + ":" + motoko.CallBackPort, "0");
                    });

                    //==================================================

                    if (InvokeRequired)
                        Invoke(act);
                    else
                        act();
                }
                else
                {
                    Action act = new Action(() =>
                    {
                        send_message_text_box.Enabled = false;
                        send_message_text_box.Text = "Пользователь вне сети..";
                    });

                    if (InvokeRequired)
                        Invoke(act);
                    else
                        act();
                }
            }
            else
            {
                Action con_action = new Action(() =>
                {
                    if (con)
                    {
                        send_message_text_box.Text = "";
                        send_message_text_box.Enabled = true;
                    }
                    else
                    {
                        send_message_text_box.Enabled = false;
                        send_message_text_box.Text = "Пользователь вне сети..";
                    }
                });

                if (!group)
                    if (InvokeRequired)
                        Invoke(con_action);
                    else
                        con_action();
            }
        }
        private void display_code_zero(string message)
        {
            Action act = new Action(() =>
            {
                string global_name = message.Split('`')[2];
                string ip = message.Split('`')[1];

                TreeNode parent_node = get_parent_node(global_name);

                if (parent_node == null)
                {
                    TreeNode node_ = new TreeNode(get_name_by_global_name(global_name), 3, 3)
                    {
                        Tag = global_name
                    };
                    treeView1.Nodes.Add(node_);

                    parent_node = get_parent_node(global_name);

                    TreeNode new_node = new TreeNode(ip, 2, 2)
                    {
                        Tag = ip
                    };

                    if (InvokeRequired)
                        Invoke(new Action(() =>
                        {
                            parent_node.ImageIndex = 3;
                            parent_node.Nodes.Add(new_node);
                            parent_node.SelectedImageIndex = parent_node.ImageIndex;
                        }));
                    else
                    {
                        parent_node.ImageIndex = 3;
                        parent_node.Nodes.Add(new_node);
                        parent_node.SelectedImageIndex = parent_node.ImageIndex;
                    }
                }
                else
                {
                    bool exist = false;
                    foreach (TreeNode node in parent_node.Nodes)
                    {
                        if (node.Tag != null)
                            if (node.Tag.ToString() == ip)
                            {
                                exist = true;
                                break;
                            }
                    }

                    if (!exist)
                    {
                        TreeNode new_node = new TreeNode(ip, 2, 2)
                        {
                            Tag = ip
                        };
                        //new_node.SelectedImageIndex = new_node.ImageIndex;

                        if (InvokeRequired)
                            Invoke(new Action(() => parent_node.Nodes.Add(new_node)));
                        else
                            parent_node.Nodes.Add(new_node);
                    }
                }

                create_table(global_name);

                DataSet ds = sqlite.Select_DataSet("SELECT open_key FROM Keys WHERE global_name LIKE '" + global_name + "'");

                if (ds.Tables[0].Rows.Count == 0)
                {

                    motoko.remoteEP = new IPEndPoint(IPAddress.Parse(ip.Split(':')[0]), Convert.ToInt32(ip.Split(':')[1]));

                    string key = get_public_key_by_blobal_name(global_name);
                    motoko.user_public_key = key;
                    if (key != null)
                    {
                        motoko.connection();

                        _connectionDone.WaitOne();

                        motoko.crypted_send_message(motoko.CallBackIPAddress + ":" +
                            motoko.CallBackPort + "`" + my_global_name, "4");
                    }
                }
            });

            if (InvokeRequired)
                Invoke(act);
            else
                act();
        } //0
        private void display_code_second(string message)
        {
            Action act = new Action(() =>
            {
                string address = message.Split('`')[1];
                //MessageBox.Show("=>  " + message);
                string data = message.Split('`')[2];
                string global_name = message.Split('`')[3];

                create_table(global_name);

                sqlite.ExecuteRequest("INSERT INTO '" + global_name + "' " +
                    "VALUES (NULL, '" + address + "', '" + DateTime.Now + "','" + data + "', NULL)");

                if (current_connection == address)
                {
                    DataSet ds = sqlite.Select_DataSet("SELECT id FROM '" + global_name + "' ORDER BY id DESC LIMIT 1");

                    object[] row = new object[5];

                    if (ds != null)
                        row[0] = ds.Tables[0].Rows[0].ItemArray[0].ToString();
                    else
                        row[0] = "1";

                    row[1] = address;
                    string date = DateTime.Now.ToString().Split(' ')[1];
                    row[2] = date.Split(':')[0] + ":" + date.Split(':')[1];
                    row[3] = data;

                    //if (dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[1])

                    message_view_dgv.Rows.Insert(message_view_dgv.Rows.Count, row);
                    message_view_dgv.Rows[message_view_dgv.Rows.Count - 1].Cells[3].Style.Alignment =
                                                            DataGridViewContentAlignment.MiddleLeft;

                    message_view_dgv.FirstDisplayedScrollingRowIndex = message_view_dgv.RowCount - 1;
                }
                else
                {
                    mark_node(global_name, address);
                }
            });

            if (InvokeRequired)
                this.Invoke(act);
            else
                act();
        } //2
        private void display_code_twenty_first(string message)
        {
            Action act = new Action(() =>
            {
                string address = message.Split('`')[1];
                string data = message.Split('`')[2];
                string sign = message.Split('#')[1];
                string global_name = message.Split('`')[message.Split('`').Length - 2];

                create_table(global_name);

                sqlite.ExecuteRequest("INSERT INTO '" + global_name + "' " +
                    "VALUES (NULL, '" + address + "', '" + DateTime.Now + "','" + data + "', '" + sign + "')");

                if (current_connection == address)
                {
                    DataSet ds = sqlite.Select_DataSet("SELECT id FROM '" + global_name + "' ORDER BY id DESC LIMIT 1");

                    object[] row = new object[5];

                    if (ds != null)
                        row[0] = ds.Tables[0].Rows[0].ItemArray[0].ToString();
                    else
                        row[0] = "1";

                    row[1] = address;
                    string date = DateTime.Now.ToString().Split(' ')[1];
                    row[2] = date.Split(':')[0] + ":" + date.Split(':')[1];
                    row[3] = data;
                    row[4] = sign;
                    //if (dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[1])

                    message_view_dgv.Rows.Insert(message_view_dgv.Rows.Count, row);
                    message_view_dgv.Rows[message_view_dgv.Rows.Count - 1].Cells[3].Style.Alignment =
                                                            DataGridViewContentAlignment.MiddleLeft;
                    message_view_dgv.Rows[message_view_dgv.Rows.Count - 1].Cells[3].Style.Font =
                                                        message_view_dgv.RowsDefaultCellStyle.Font;

                    message_view_dgv.Rows[message_view_dgv.Rows.Count - 1].Cells[3].Style.ForeColor =
                        Color.FromArgb(120, 198, 200);

                    message_view_dgv.FirstDisplayedScrollingRowIndex = message_view_dgv.RowCount - 1;
                }
                else
                {
                    mark_node(global_name, address);
                }
            });

            if (InvokeRequired)
                this.Invoke(act);
            else
                act();
        } // 21
        private TreeNode search_existing_groups(string node_tag)
        {
            TreeNode check_in(TreeNode node)
            {
                foreach (TreeNode _node in node.Nodes)
                {
                    if (_node.Tag != null)
                        if (_node.Tag.ToString() == node_tag)
                            return _node;
                    
                    if (_node.Nodes.Count > 0)
                    {
                        TreeNode node_ = check_in(_node);
                        if (node_ != null)
                            return node_;
                    }
                }

                return null;
            }

            foreach (TreeNode node in treeView1.Nodes)
            {
                if (node.Tag != null)
                    if (node.Tag.ToString() == node_tag)
                        return node;
                
                if (node.Nodes.Count > 0)
                {
                    TreeNode node_ = check_in(node);
                    if (node_ != null)
                        return node_;
                }
            }

            return null;
        }
        private TreeNode searching_node_in_node_container(TreeNode node, string node_tag)
        {
            foreach (TreeNode _node in node.Nodes)
            {
                if (_node.Tag.ToString() == node_tag)
                {
                    return _node;
                }
            }

            return null;
        }
        private void create_user(TreeNode group_node, string global_name)
        {
            Action act = new Action(() =>
            {
                TreeNode user_node = new TreeNode(get_name_by_global_name(global_name), 3, 3)
                {
                    Tag = global_name
                };

                group_node.Nodes.Add(user_node);

                if (!sqlite.Check_table_exist(global_name))
                {
                    sqlite.ExecuteRequest("CREATE TABLE '" + global_name + "'(" +
                                                                    "id INTEGER PRIMARY KEY AUTOINCREMENT, " +
                                                                    "sender VARCHAR(30), " +
                                                                    "date VARCHAR(30), " +
                                                                    "data TEXT, " +
                                                                    "sign TEXT)");
                }
            });

            if (InvokeRequired) Invoke(act);
            else act();
        }
        private void create_group(string group_name)
        {
            Action act = new Action(() =>
            {
                TreeNode group_node = new TreeNode(group_name, 4, 4)
                {
                    Tag = group_name
                };

                treeView1.Nodes.Add(group_node);
                if (!sqlite.Check_table_exist(group_name))
                {
                    sqlite.ExecuteRequest("CREATE TABLE '" + group_name + "'(" +
                           "id INTEGER PRIMARY KEY AUTOINCREMENT, " +
                           "sender VARCHAR(65), " +
                           "date VARCHAR(30), " +
                           "data TEXT, " +
                           "sign TEXT)");
                }
            });

            if (InvokeRequired) Invoke(act);
            else act();
        }
        private void create_address(TreeNode user_node, string address)
        {
            Action act = new Action(() =>
            {
                TreeNode group_node = new TreeNode(address, 2, 2)
                {
                    Tag = address
                };

                user_node.Nodes.Add(group_node);
            });

            if (InvokeRequired) Invoke(act);
            else act();
        }
        private void display_code_forth(string message) //4
        {
            string group = message.Split('`')[1];
            string IP = message.Split('`')[2].Split(':')[0];
            string port = message.Split('`')[2].Split(':')[1];
            string msg = message.Split('`')[3];
            string global_name = message.Split('`')[4];
            string user_custom_name = get_name_by_global_name(global_name);


            TreeNode node = search_existing_groups(group);
            TreeNode user;
            TreeNode address;

            if (node != null)
            {
                user = searching_node_in_node_container(node, global_name);
                if (user != null)
                {
                    address = searching_node_in_node_container(user, IP + ":" + port);
                    if (address != null)
                    {
                        ;
                    }
                    else
                    {
                        create_address(user, IP + ":" + port);
                    }
                }
                //else
                //{
                //    create_user(node, global_name);
                //    user = searching_node_in_node_container(node, global_name);

                //    create_address(user, IP + ":" + port);
                //}
            }
            else
            {
                create_group(group);
                node = search_existing_groups(group);

                create_user(node, global_name);
                user = searching_node_in_node_container(node, global_name);

                create_address(user, IP + ":" + port);
            }

            node = search_existing_groups(group);
            if (searching_node_in_node_container(node, global_name) != null)
            {
                sqlite.ExecuteRequest("INSERT INTO `" + group + "` VALUES (NULL, '" + global_name +
                    "', '" + DateTime.Now + "', '" + msg + "', NULL)");
            }

            if (current_connection == group)
            {
                DataSet ds = sqlite.Select_DataSet("SELECT id FROM '" + global_name + "' ORDER BY id DESC LIMIT 1");

                object[] row = new object[5];

                if (ds != null)
                    row[0] = ds.Tables[0].Rows[0].ItemArray[0].ToString();
                else
                    row[0] = "1";

                row[1] = global_name;
                string date = DateTime.Now.ToString().Split(' ')[1];
                row[2] = date.Split(':')[0] + ":" + date.Split(':')[1];
                row[3] = msg;

                //if (dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[1])

                message_view_dgv.Rows.Insert(message_view_dgv.Rows.Count, row);
                message_view_dgv.Rows[message_view_dgv.Rows.Count - 1].Cells[3].Style.Alignment =
                                                        DataGridViewContentAlignment.MiddleLeft;

                message_view_dgv.FirstDisplayedScrollingRowIndex = message_view_dgv.RowCount - 1;
            }
            else
            {
                ;//Marking node group 
            }

            TreeNode[] treeNodes = new TreeNode[0];

            foreach (TreeNode _node in node.Nodes)
            {
                if (_node.Nodes.Count > 0)
                {
                    check_in(_node, ref treeNodes, 2);
                }
            }

            foreach (TreeNode _node in treeNodes)
            {
                if (_node.Tag.ToString() != (IP + ":" + port))
                {
                    string ip = _node.Tag.ToString().Split(':')[0];
                    int port_ = Convert.ToInt32(_node.Tag.ToString().Split(':')[1]);

                    motoko.remoteEP = new IPEndPoint(IPAddress.Parse(ip), port_);
                    motoko.user_public_key = get_public_key_by_blobal_name(_node.Parent.Tag.ToString());

                    //send_message_text_box.Text = "Подключение...";
                    //send_message_text_box.Enabled = false;

                    motoko.connection();
                    //Ты кароче бездарный долбаеб
                    //Коннект должен быть в отельной функции в отдельном потоке и после
                    //подлючения должен тригерить делигат
                    //который уже после одключения будет отправлять твое дерьмо 
                    //как я только что понял у тебя еблана есть такой подход
                    //Но какого хуя ты думал что в остальных местах так не надо делать?
                    //В локалочке-то оно ахуенно, слип на 0.2 секи въебал и в хуй не дуешь?
                    //А в интернетах ты думаешь твоя хуета будут конектится за 0.2 секундны?
                    //Еще и поток отключает. мудак? нет бы подумать и придумать что-то не такое
                    //топорное

                    Thread.Sleep(200);

                    //send_message_text_box.Text = "Отправка...";
                    //send_message_text_box.Enabled = false;

                    if (sending_to_group(msg, my_global_name, current_dialog))
                    {
                        //there u can handle every sended message
                    }

                    //send_message_text_box.Text = "";
                    //send_message_text_box.Enabled = true;
                }
            }
        }
        private void display_code_fifth(string message)
        {
            string user_public_key = message.Split('`')[1] + "";
            string global_name = message.Split('`')[2] + "";
            DataSet ds = sqlite.Select_DataSet("SELECT open_key FROM Keys WHERE global_name LIKE '" + global_name + "'");

            if (ds.Tables[0].Rows.Count == 0)
            {
                sqlite.ExecuteRequest("INSERT INTO Keys VALUES (NULL, '" + global_name + "', '" + user_public_key + "')");
            }
        } //5
        private void display_code_sixth(string message) // 6
        {
            string IP = message.Split('`')[1].Split(':')[0];
            string port = message.Split('`')[1].Split(':')[1];
            string file_name = message.Split('`')[2];

            //if (InvokeRequired) Invoke(new Action(() => textBox3.Text = message.Split('`')[3])); else textBox3.Text = message.Split('`')[3];
            byte[] file = Convert.FromBase64String(message.Split('`')[3]);
            string global_name = message.Split('`')[4];

            MessageBox.Show("Address is " + IP + ":" + port + "\n" +
                            "file name is " + file_name + "\n" +
                            "file size is " + file.Length + "\n" +
                            "global name is " + global_name
                );
            File.WriteAllBytes("./files/" + file_name, file);
        }
        private void Display(string message)
        {
            if (message.Split('`')[0] == "0")
            {
                display_code_zero(message);
            }
            else if (message.Split('`')[0] == "2")
            {
                display_code_second(message);
            }
            else if (message.Split('`')[0] == "21")
            {
                display_code_twenty_first(message);
            }
            else if (message.Split('`')[0] == "4")
            {
                display_code_forth(message);
            }
            else if (message.Split('`')[0] == "5")
            {
                display_code_fifth(message);
            }
            else if (message.Split('`')[0] == "6")
            {
                display_code_sixth(message);
            }

            Action act = new Action(() =>
            {
                label8.Text = "0";
            });

            if (InvokeRequired)
                Invoke(act);
            else act();

        }
        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (group)
            {
                send_to_group(e);
            }
            else
                send_to_user(e);
        }
        //===================================[Buttons]======================================
        private void button1_Click(object sender, EventArgs e)
        {
            if (!conWindowAnimation.IsAlive)
            {
                if (treeView1.Location.Y > 55 + 26)
                {
                    add_user_panel.BackgroundImage = Image.FromFile(@"./images/ico/treeView/user.png");
                    conWindowAnimation = new Thread(new ThreadStart(conWinAnimation));
                    conWindowAnimation.Start();
                }
            }
        }
        private string get_name_by_global_name(string global_name)
        {
            DataSet ds = sqlite.Select_DataSet("SELECT name FROM names WHERE global_name LIKE '" + global_name + "'");

            if (ds.Tables[0] != null & ds.Tables[0].Rows.Count > 0)
            {
                return ds.Tables[0].Rows[0].ItemArray[0].ToString();
            }

            return global_name;
        }
        private void Node_add_Button_Click(object sender, EventArgs e)
        {
            void method()
            {
                try
                {
                    IPAddress ip = IPAddress.Parse(address_textbox.Text.Split(':')[0]);
                    int port = Int32.Parse(address_textbox.Text.Split(':')[1]);

                    //Bing user address
                    motoko.remoteEP = new IPEndPoint(ip, port);
                    motoko.global_name = global_name_textbox.Text;

                    try
                    {
                        DataSet ds = sqlite.Select_DataSet("SELECT open_key FROM Keys WHERE global_name LIKE '" + global_name_textbox.Text + "'");

                        if (ds.Tables[0].Rows.Count == 0)
                        {
                            sqlite.ExecuteRequest("INSERT INTO Keys VALUES (NULL, '" + global_name_textbox.Text + "', '" + open_key_textbox.Text + "')");
                        }

                        motoko.user_public_key = get_public_key_by_blobal_name(global_name_textbox.Text);
                        //Connecting to user
                        motoko.first_connection();

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString() + "\n\n" + ex.Message);
                    }
                }
                catch
                {
                    //INCORRECT IP
                }
            }

            if (global_name_textbox.Text.Length == 64 && address_textbox.Text != "" && open_key_textbox.Text != "")
            {

                send_message_text_box.Text = "Подключение...";
                send_message_text_box.Enabled = false;

                conn = new Thread(new ThreadStart(method));
                //Add new address in list (treeView1) by button
                if (motoko.CallBackIPAddress == null)
                    conn.Start();
                else if (motoko.CallBackIPAddress.Address.ToString() != address_textbox.Text)
                    conn.Start();
            }
        }
        //==========================[TreeView ContextMenu Events]===========================
        //////===============================[Animation]=========================
        private void conWinAnimation()
        {
            int i = 0;
            int height = panel1.Height;

            Action act_show = new Action(() =>
            {
                treeView1.Height -= 1;
                treeView1.Top += 1;
            });

            Action act_hide = new Action(() =>
            {
                treeView1.Height += 1;
                treeView1.Top -= 1;
            });

            if (treeView1.Location.Y == 55 + 26)
            {
                for (i = 11; i <= 90 + 33 + win_anim_offset; i++)
                {
                    try
                    {
                        if (InvokeRequired)
                            this.Invoke(act_show);
                        else
                            act_show();
                        Thread.Sleep(10);
                    }
                    catch (ObjectDisposedException)
                    {
                        break;
                    }
                }
            }
            else
            {
                for (i = 90 + 33 + win_anim_offset; i > 10; i--)
                {
                    try
                    {
                        if (InvokeRequired)
                            this.Invoke(act_hide);
                        else
                            act_hide();
                        Thread.Sleep(10);
                    }
                    catch
                    {
                        break;
                    }
                }
            }
            //conWindowAnimation.Abort();
        }
        private void new_connection_window_showing(object sender, EventArgs e)
        {
            if (!conWindowAnimation.IsAlive)
            {
                if (treeView1.Location.Y == 55 + 26)
                {
                    Node_add_Button.Visible = true;
                    address_textbox.Visible = true;
                    button2.Visible = false;
                    open_key_textbox.Visible = true;

                    address_textbox.Text = Lang[15];
                    address_textbox.ForeColor = Color.Silver;
                    global_name_textbox.Text = Lang[16];
                    global_name_textbox.ForeColor = Color.Silver;
                    open_key_textbox.Text = "Публичный ключ";
                    open_key_textbox.ForeColor = Color.Silver;

                    add_user_panel.BackgroundImage = Image.FromFile(@"./images/ico/treeView/user_press.png");

                    conWindowAnimation = new Thread(new ThreadStart(conWinAnimation));
                    conWindowAnimation.Start();
                }
                else
                {
                    add_user_panel.BackgroundImage = Image.FromFile(@"./images/ico/treeView/user.png");

                    conWindowAnimation = new Thread(new ThreadStart(conWinAnimation));
                    conWindowAnimation.Start();
                }
            }
        }
        private void rename_node_window_showing(object sender, EventArgs e)
        {
            if (treeView1.Location.Y == 55 + 26)
            {
                renaming_node = treeView1.SelectedNode;
                global_name_textbox.Text = renaming_node.Text;
                global_name_textbox.ForeColor = Color.Black;//Color.FromArgb(120, 198, 200);

                if (renaming_node.ImageIndex == 3) //user
                {
                    address_textbox.Text = renaming_node.Tag.ToString();

                    DataSet ds = sqlite.Select_DataSet("SELECT open_key FROM Keys WHERE global_name LIKE '" + global_name_textbox.Text + "'");
                    if (ds.Tables[0].Rows.Count > 0)
                        open_key_textbox.Text = ds.Tables[0].Rows[0].ItemArray[0].ToString();

                    address_textbox.Visible = true;
                    address_textbox.ReadOnly = true;

                    open_key_textbox.Visible = true;
                    open_key_textbox.ReadOnly = true;

                    button1.Location = new Point(2, 85); // 85
                    button2.Location = new Point(button1.Width + 12, 85);
                    Node_add_Button.Location = new Point(button1.Width + 12, 85);

                    win_anim_offset = 0;
                }
                else if (renaming_node.ImageIndex == 2 || renaming_node.ImageIndex == 4) //address
                {
                    address_textbox.Text = renaming_node.Tag.ToString();
                    address_textbox.Visible = true;
                    address_textbox.ReadOnly = true;

                    open_key_textbox.Visible = false;

                    button1.Location = new Point(2, 85 - 26); // 85
                    button2.Location = new Point(button1.Width + 12, 85 - 26);
                    Node_add_Button.Location = new Point(button1.Width + 12, 85 - 26);

                    win_anim_offset = -26;
                }
                else if (renaming_node.ImageIndex == 1) // folder
                {
                    address_textbox.Visible = false;
                    open_key_textbox.Visible = false;

                    button1.Location = new Point(2, 85 - 50); // 85
                    button2.Location = new Point(button1.Width + 12, 85 - 50);
                    Node_add_Button.Location = new Point(button1.Width + 12, 85 - 50);

                    win_anim_offset = -50;
                }

                Node_add_Button.Visible = false;
                button2.Visible = true;

                conWindowAnimation = new Thread(new ThreadStart(conWinAnimation));
                conWindowAnimation.Start();
            }
            else
            {
                conWindowAnimation = new Thread(new ThreadStart(conWinAnimation));
                conWindowAnimation.Start();
            }
        }
        //////===================================================================
        private void Add_MenuItem_click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode == null)
            {
                treeView1.Nodes.Add("0", Lang[14], 1, 1);
            }
            else if (treeView1.SelectedNode.ImageIndex == 1)
            {
                TreeNode newNode = new TreeNode(Lang[14], 1, 1);
                treeView1.SelectedNode.Nodes.Add(newNode);
                treeView1.SelectedNode.Expand();
            }
        }
        private void rename_node_button_click(object sender, EventArgs e)
        {
            if (global_name_textbox.Text != "")
            {
                renaming_node.Text = global_name_textbox.Text;

                conWindowAnimation = new Thread(new ThreadStart(conWinAnimation));
                conWindowAnimation.Start();

                if (renaming_node.Tag != null)
                {
                    string global_name = renaming_node.Tag.ToString();

                    DataSet ds = sqlite.Select_DataSet("SELECT id FROM names WHERE global_name LIKE '" + global_name + "'");

                    if (ds.Tables[0].Rows != null & ds.Tables[0].Rows.Count > 0)
                    {
                        string id = ds.Tables[0].Rows[0].ItemArray[0].ToString();
                        sqlite.ExecuteRequest("UPDATE names SET name = '" + renaming_node.Text + "' WHERE id LIKE " + id + " ");

                    }
                    else
                    {
                        sqlite.ExecuteRequest("INSERT INTO names VALUES (NULL, '" + renaming_node.Tag + "', '" + renaming_node.Text + "')");
                    }
                }
                address_textbox.Text = Lang[15];
                address_textbox.ForeColor = Color.Silver;
                global_name_textbox.Text = Lang[16];
                global_name_textbox.ForeColor = Color.Silver;
            }
        }
        private void Delete_all_MenuItem_click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode != null)
                treeView1.SelectedNode.Nodes.Clear();
            treeView1.Nodes.Remove(treeView1.SelectedNode);
        }
        private void Delete_MenuItem_click(object sender, EventArgs e)
        {
            treeView1.SelectedNode.Nodes.Clear();
        }
        //==========================[TreeView additional methods]===========================
        delegate void TreeViewCallBack(string ip);
        public void TreeViewNodes(string ip)
        {

            if (InvokeRequired) // Вызов метода из другого потока
            {
                TreeViewCallBack callBack = new TreeViewCallBack(TreeViewNodes);
                Invoke(callBack, new object[] { ip });
            }
            else // Вызов метода из текущего потока
            {
                TreeNode node = new TreeNode(ip, 2, 2)
                {
                    Tag = ip
                };
                treeView1.Nodes.Add(node);
            }
        }
        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (e.Node.ImageIndex == 1 || 
                        e.Node.ImageIndex == 3 || 
                            e.Node.ImageIndex == 4)
                {
                    if (e.Node.IsExpanded)
                        e.Node.Collapse();
                    else
                        e.Node.Expand();
                }
                else if (e.Node.ImageIndex == 2)
                {
                    label1.Text = e.Node.Text;
                    autosend_textbox.Enabled = true;
                }
            }

            if (e.Button == MouseButtons.Right)
            {
                ContextMenu m = new ContextMenu();

                TreeNode node = treeView1.HitTest(e.X, e.Y).Node;

                if (node.ImageIndex == 1)
                {
                    m.MenuItems.Add(Lang[10]);
                    m.MenuItems.Add(Lang[11]);
                    m.MenuItems.Add(Lang[12]);
                    m.MenuItems.Add(Lang[13]);

                    m.MenuItems[0].Click += new EventHandler(Add_MenuItem_click);
                    m.MenuItems[1].Click += new EventHandler(rename_node_window_showing);
                    m.MenuItems[2].Click += new EventHandler(Delete_all_MenuItem_click);
                    m.MenuItems[3].Click += new EventHandler(Delete_MenuItem_click);
                }
                else if (node.ImageIndex == 2)
                {
                    m.MenuItems.Add(Lang[11]);
                    m.MenuItems.Add(Lang[12]);
                    m.MenuItems[0].Click += new EventHandler(rename_node_window_showing);
                    m.MenuItems[1].Click += new EventHandler(Delete_all_MenuItem_click);
                }
                else if (node.ImageIndex == 3 || node.ImageIndex == 4)
                {
                    m.MenuItems.Add("Перейти в диалог");
                    m.MenuItems.Add(Lang[11]);
                    m.MenuItems.Add(Lang[12]);
                    m.MenuItems.Add(Lang[13]);

                    m.MenuItems[0].Click += new EventHandler(show_dialog);
                    m.MenuItems[1].Click += new EventHandler(rename_node_window_showing);
                    m.MenuItems[2].Click += new EventHandler(Delete_all_MenuItem_click);
                    m.MenuItems[3].Click += new EventHandler(Delete_MenuItem_click);
                }

                m.Show(treeView1, new Point(e.X, e.Y));

                treeView1.SelectedNode = node;
            }
            e.Node.BackColor = CustomBackColor;
        }
        private void treeView1_KeyPress(object sender, KeyPressEventArgs e)
        {
            ////Connection by Enter on node
            //if (current_slider == 0)
            //    if (e.KeyChar == (char)13)
            //    {
            //        if (treeView1.SelectedNode != null)
            //            if (treeView1.SelectedNode.ImageIndex.ToString() == "2")
            //            {
            //                motoko.remoteEP = new IPEndPoint(
            //                    IPAddress.Parse(treeView1.SelectedNode.Tag
            //                        .ToString().Split(':')[0]),

            //                    Int32.Parse(treeView1.SelectedNode.Tag
            //                        .ToString().Split(':')[1])
            //                );

            //                motoko.connection();

            //                current_connection = motoko.remoteEP.Address.ToString() + ":" + motoko.remoteEP.Port.ToString();
            //            }
            //    }
        }
        private void mark_node(string global_name, string value)
        {
            void method(TreeNode _node)
            {
                _node.BackColor = Color.FromArgb(120, 198, 200);
                if (_node.Parent != null)
                    method(_node.Parent);
            }

            TreeNode node = get_parent_node(global_name);

            foreach (TreeNode n in node.Nodes)
            {
                if (n.Tag != null && n.Tag.ToString() == value)
                    n.BackColor = Color.FromArgb(120, 198, 200);
            }
            method(node);
        }
        private TreeNode get_parent_node(string global_name)
        {
            TreeNode method(TreeNode _node_)
            {
                TreeNode __node__;
                foreach (TreeNode _node in _node_.Nodes)
                {

                    if (_node.Tag != null)
                        if (_node.Tag.ToString() == global_name)
                        {
                            return _node;
                        }

                    if (_node.Nodes.Count > 0)
                    {
                        __node__ = method(_node);
                        if (__node__ != null)
                            return __node__;
                    }
                }
                return null;
            }

            //=====================================
            TreeNode __node;

            foreach (TreeNode node in treeView1.Nodes)
            {
                if (node.Tag != null)
                    if (node.Tag.ToString() == global_name)
                    {
                        return node;
                    }

                if (node.Nodes.Count > 0)
                {
                    __node = method(node);
                    if (__node != null)
                        return __node;
                }
            }

            return null;
        }
        int message_offset = 0;
        private void treeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            //connection to node by click on node in treeview
            void method()
            {
                if (e.Node.ImageIndex == 2)
                {
                    IPAddress ip = IPAddress.Parse(e.Node.Tag.ToString().Split(':')[0]);
                    int port = Int32.Parse(e.Node.Tag.ToString().Split(':')[1]);

                    motoko.remoteEP = new IPEndPoint(ip, port);
                    string global_name = null;

                    try
                    {
                        //Добавить метод о начале создания подключения

                        Action act_con = new Action(() =>
                        {
                            send_message_text_box.Enabled = false;
                            send_message_text_box.Text = "Подключение...";
                        });

                        if (InvokeRequired)
                            Invoke(act_con);
                        else
                            act_con();

                        Action act = new Action(() =>
                        {

                            current_connection = e.Node.Tag.ToString();

                            if (e.Node.Parent.ImageIndex == 3)
                            {
                                global_name = e.Node.Parent.Tag.ToString();
                            }

                            if (global_name != null)
                                if (current_dialog != global_name)
                                {
                                    treeView1.SelectedNode = e.Node.Parent;
                                    show_dialog(new object(), new EventArgs());
                                    current_dialog = global_name;
                                }

                            label2.Text = e.Node.Tag.ToString();

                            e.Node.BackColor = CustomBackColor;
                        });

                        if (InvokeRequired)
                            Invoke(act);
                        else
                            act();

                        DataSet ds = sqlite.Select_DataSet("SELECT open_key FROM Keys WHERE global_name LIKE '" + global_name + "'");

                        motoko.user_public_key = ds.Tables[0].Rows[0].ItemArray[0].ToString();
                        //Подключение
                        motoko.connection();

                        conn.Join(100);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                        conn.Join(100);
                        //conn.Abort();
                    }
                }
            }

            if (current_slider == 0)
                if (e.Button == MouseButtons.Left)
                {
                    if (treeView1.SelectedNode != null && e.Node.ImageIndex == 2)
                    {
                        try
                        {
                            conn = new Thread(new ThreadStart(method));

                            sign_message_panel.BackgroundImage = Image.FromFile(@"./images/pencil.png");
                            isSign = false;

                            if (motoko.remoteEP == null)
                                conn.Start();
                            else if (motoko.remoteEP.Address.ToString() != address_textbox.Text)
                                conn.Start();
                        }
                        catch (ArgumentOutOfRangeException ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }
                }
        }
        private bool check_on_match_in_treeView(string value)
        {
            bool check_node(TreeNode node)
            {
                for (int i = 0; i < node.Nodes.Count; i++)
                {
                    if (node.Nodes[i].ImageIndex != 4)
                        if (node.Nodes[i].Tag == null)
                        {
                            if (check_node(node.Nodes[i]))
                                return true;
                        }
                        else if (node.Nodes[i].Nodes.Count > 0)
                        {
                            if (node.Nodes[i].Tag.ToString() == value)
                                return true;
                            else if (check_node(node.Nodes[i]))
                                return true;
                        }
                        else
                            if (node.Nodes[i].Tag.ToString() == value)
                            return true;
                }

                return false;
            }

            foreach (TreeNode node in treeView1.Nodes)
            {
                if (node.ImageIndex != 4)
                    if (node.Tag == null)
                    {
                        if (check_node(node))
                            return true;
                    }
                    else if (node.Nodes.Count > 0)
                    {
                        if (node.Tag.ToString() == value)
                            return true;
                        else if (check_node(node))
                            return true;
                    }
                    else
                        if (node.Tag.ToString() == value)
                        return true;
            }


            return false;
        }
        private void serialize_tree_view()
        {
            TreeViewSerializer serializer = new TreeViewSerializer();
            serializer.SerializeTreeView(this.treeView1, "./components/treeView/tree.xml");

            sqlite.Insert_BLOB("UPDATE treeViews SET tree = @file WHERE name LIKE 'book'");
        }
        private void sign_message_panel_Click(object sender, EventArgs e)
        {
            if (isSign)
            {
                isSign = false;
                sign_message_panel.BackgroundImage = Image.FromFile(@"./images/pencil.png");
            }
            else
            {
                isSign = true;
                sign_message_panel.BackgroundImage = Image.FromFile(@"./images/pencil_press.png");
            }
        }
        private void message_view_dgv_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                if (e.ColumnIndex == 3 && message_view_dgv.Rows[e.RowIndex].Cells[3].Style.ForeColor != Color.White)
                {
                    string username = current_dialog;
                    int id = Convert.ToInt32(message_view_dgv.Rows[e.RowIndex].Cells[0].Value.ToString());
                    string Key;

                    DataSet ds = sqlite.Select_DataSet("SELECT * FROM '" + username + "' WHERE id LIKE " + id + " ");

                    if (ds.Tables[0].Rows[0].ItemArray[1].ToString() == "")
                        Key = public_key;
                    else
                    {
                        DataSet key_ = sqlite.Select_DataSet("SELECT open_key FROM Keys WHERE global_name LIKE '" + username + "'");
                        Key = key_.Tables[0].Rows[0].ItemArray[0].ToString();
                    }

                    string sign = ds.Tables[0].Rows[0].ItemArray[4].ToString();
                    string message = ds.Tables[0].Rows[0].ItemArray[3].ToString();
                    string date = ds.Tables[0].Rows[0].ItemArray[2].ToString();

                    SignedMessageShow form = new SignedMessageShow(Key, username, sign, message, date);
                    form.Show();
                }
        }
        private void add_group_panel_Click(object sender, EventArgs e)
        {
            Random rnd = new Random();
            int id = rnd.Next(100000000, 999999999);

            while (true)
            {
                if (sqlite.Check_table_exist(id.ToString()))
                    id = rnd.Next(100000000, 999999999);
                else
                {
                    sqlite.ExecuteRequest("CREATE TABLE '" + id.ToString() + "'(" +
                                           "id INTEGER PRIMARY KEY AUTOINCREMENT, " +
                                           "sender VARCHAR(65), " +
                                           "date VARCHAR(30), " +
                                           "data TEXT, " +
                                           "sign TEXT)");
                
                    break;
                }
            }

            TreeNode node = new TreeNode(id.ToString(), 4, 4)
            {
                Tag = id.ToString()
            };
            treeView1.Nodes.Add(node);


        }
        private void button3_Click(object sender, EventArgs e)
        {
            motoko.listener.Close();
            motoko = new Motoko(local_ip, Convert.ToInt32(textBox4.Text));
            textBox2.Text = local_ip + ":" + motoko.CallBackPort;
            receive.Abort();
            receive.Join(500);
            receive = new Thread(new ThreadStart(motoko.StartListening));
            motoko.GetMessageHandler(Display);
            motoko.GetConnectionHandler(connect_result);
            autosend.GetConnectionHandler(autosend_result);
            //motoko_as.GetConnectionHandler(autosend_result);
            receive.Start();
        }
        private void button4_Click(object sender, EventArgs e)
        {
            string path = @"./DataBases/DataBase.db3";
            File.Delete(path);
            DataBase db = new DataBase();
            db.create_new_data_base();

            DataSet ds = sqlite.Select_DataSet("SELECT * FROM user_info");
            my_global_name = ds.Tables[0].Rows[0].ItemArray[0].ToString();
            public_key = ds.Tables[0].Rows[0].ItemArray[1].ToString();
            private_key = ds.Tables[0].Rows[0].ItemArray[2].ToString();

            textBox1.Text = my_global_name;
            textBox3.Text = public_key;
        }
        Thread send_file;
        private void file_send()
        {
            var open = new OpenFileDialog();

            Action act = new Action(() =>
            {
                open.ShowDialog();
            });

            if (InvokeRequired)
                Invoke(act);
            else
                act();


            string path = open.FileName;
            string file_name = open.SafeFileName;

            byte[] file = File.ReadAllBytes(path);
            string message = Convert.ToBase64String(file);

            motoko.connection();

            Thread.Sleep(200);

            string text;

            decimal d = Convert.ToDecimal(message.Length) / 100;


            if (message != "")
            {
                for (int i = 0; i < Math.Ceiling(d + 1); i++)
                {
                    try
                    {
                        Action act1 = new Action(() =>
                        {
                            send_message_text_box.Enabled = false;
                            send_message_text_box.Text = "Отправка файла " + i + " частей из " + Math.Round(d + 1) + ".";
                        });

                        if (InvokeRequired)
                            Invoke(act1);
                        else
                            act1();
                        Thread.Sleep(100);


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

                            motoko.crypted_send_message(
                                motoko.CallBackIPAddress + ":" +
                                motoko.CallBackPort + "`" + file_name + "`", "6");
                            motoko.crypted_send_message(text, "3");

                        }
                        else
                        {
                            motoko.crypted_send_message(text, "3");
                        }

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                        return;
                    }
                }
                motoko.crypted_send_message("`" + my_global_name + "`#", "3");
            }
            motoko.crypted_send_message(motoko.CallBackIPAddress + ":" +
                                motoko.CallBackPort + "`" + "Отправил файл: " + file_name, "2");
            motoko.crypted_send_message("`" + my_global_name + "`#", "3");

            file_sending.WaitOne();

            Action act2 = new Action(() =>
            {
                send_message_text_box.Text = "";
                send_message_text_box.Enabled = true;
            });

            if (InvokeRequired)
                Invoke(act2);
            else
                act2();

        }
        private void file_send_panel_Click(object sender, EventArgs e)
        {
            send_file = new Thread(new ThreadStart(file_send));
            send_file.Start();
        }
        //==================================================================================
        private void increment_pockets()
        {
            Action act = new Action(() => 
            {  
                label8.Text = "" + (Convert.ToInt32(label8.Text) + 1);
            });

            if (InvokeRequired)
                Invoke(act);
            else act();
        }

        private void panel1_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {
            textBox4.Visible = true;
            button3.Visible = true;
            button4.Visible = true;
        }

        private void help_panel_Click(object sender, EventArgs e)
        {
            Process.Start(@".\site\index.html");
        }
    }
}

//0 - подключение (connection)
//1 - подписанное сообщение  
//2 - отправка сообщения (message)
//3 - продолжение сообщения или его конец
//5 - получение ключа
