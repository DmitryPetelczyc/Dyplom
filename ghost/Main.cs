using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace ghost
{
    public partial class Main : Form
    {
        SQLite sqlite = new SQLite();

        //thread for receiving messages
        Thread receive;
        Thread conWindowAnimation;

        static Color CustomForeColor = Color.White;//Color.FromArgb(120, 198, 200);
        static Color FormBackColor = Color.FromArgb(29, 29, 36);
        static Color CustomBackColor = Color.FromArgb(35, 35, 45);

        //set massive for words (current language)
        public static string[] Lang = new string[100];


        //===============================[Language Setings]=================================
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

            infoToolStripMenuItem.Text = Lang[0];
            label2.Text = Lang[1];
            myIPAddressToolStripMenuItem.Text = Lang[2];
            Node_add_Button.Text = Lang[3];
            //Send_message_Button.Text = Lang[4];
            iPAddressToolStripMenuItem.Text = Lang[7];
            button1.Text = Lang[18];
            button2.Text = Lang[11];

            textBox2.Text = Lang[15];
            textBox2.ForeColor = Color.Silver;
            textBox3.Text = Lang[16];
            textBox3.ForeColor = Color.Silver;
        }
        //==================================================================================
        public Main()
        {
            InitializeComponent();
            LangInit();
            this.FormBorderStyle = FormBorderStyle.None;
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.ResizeRedraw, true);


            //get local ip address
            info.MyIP = IPAddress.Parse(Weapons.GetLocalIP());
            //find a free port
            info.MyPort = 3000;//Weapons.GetPort();//54321;

            try
            {
                //set local ip address + port in menu 
                nullLocalIPToolStripMenuItem.Text =
                    info.MyIP.ToString() + ":" + info.MyPort.ToString();
                nullLocalIPToolStripMenuItem.ForeColor = Color.Green;

                //set ip address + port in menu
                string ip = Weapons.GetIP();//"null";// = Weapons.GetIP();
                nullIPToolStripMenuItem.Text = ip + ":" + info.MyPort.ToString();
                nullIPToolStripMenuItem.ForeColor = Color.Green;
            }
            catch (System.Net.WebException ex)
            {
                nullIPToolStripMenuItem.Text = Lang[1];
                MessageBox.Show(ex.ToString());
            }

            try
            {
                //Set thread for receiving messages
                Listener listn = new Listener();
                receive = new Thread(new ThreadStart(listn.StartListening));
                //Collect received massages in richtextbox1 
                //and set method to delegate 
                listn.GetMessageHandler(Display);
                Connection.SetConnectionHandler(Display);

                receive.Start();

                nullLocalIPToolStripMenuItem.ForeColor = Color.Green;
            }
            catch (Exception ex)
            {
                nullLocalIPToolStripMenuItem.ForeColor = Color.Red;
                MessageBox.Show(ex.ToString());
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            void Set_SelectedImageIndex_in_treeView_like_IamgeIndex(TreeNode node)
            {
                node.SelectedImageIndex = node.ImageIndex;
                if (node.Nodes.Count > 0)
                {
                    for (int i = 0; i < node.Nodes.Count; i++)
                    {
                        node.Nodes[i].SelectedImageIndex = node.Nodes[i].ImageIndex;
                        if (node.Nodes[i].Nodes.Count > 0)
                        {
                            Set_SelectedImageIndex_in_treeView_like_IamgeIndex(node.Nodes[i]);
                        }
                    }
                }
            }

            //Creating new database if db not exist
            if (!File.Exists(@"./DataBases/DataBase.db3"))
            {
                DataBase DB = new DataBase();
                DB.create_new_data_base();
            }

            //this.SuspendLayout();
            panel3.SuspendLayout();

            //===============================[treeView Settings]================================
            {
                string path = "./images/ico/treeView/";
                ImageList imglist = new ImageList();
                imglist.ImageSize = new Size(21, 21);

                imglist.Images.Add("1", Image.FromFile(path + "void.png"));
                imglist.Images.Add("2", Image.FromFile(path + "folder.png"));
                imglist.Images.Add("3", Image.FromFile(path + "laptop.png"));

                treeView1.ImageList = imglist;
                treeView1.BorderStyle = BorderStyle.None;
                treeView1.LineColor = Color.Gray;
                treeView1.ShowLines = false;
                treeView1.ShowPlusMinus = false;
                treeView1.BackColor = this.BackColor;
                treeView1.AllowDrop = true;
                treeView1.BackColor = Color.Black;
                treeView1.Font = new Font("Aire Exterior", 12);

                //treeView1.Anchor = panel2.Anchor;


                treeView1.ItemDrag += new ItemDragEventHandler(treeView1_ItemDrag);
                treeView1.DragEnter += new DragEventHandler(treeView1_DragEnter);
                treeView1.DragOver += new DragEventHandler(treeView1_DragOver);
                treeView1.DragDrop += new DragEventHandler(treeView1_DragDrop);
            }
            //==================================================================================
            SQLite sqlite = new SQLite();
            sqlite.Connect(@"./DataBases/DataBase.db3");

            DataSet ds = sqlite.Select_DataSet("SELECT * FROM treeViews"); //  WHERE name LIKE 'test'
            byte[] buffer;
            if (ds.Tables[0].Rows[ds.Tables[0].Rows.Count - 1].ItemArray[2] != null)
            {
                buffer = (byte[])ds.Tables[0].Rows[ds.Tables[0].Rows.Count - 1].ItemArray[2];

                var reader = new StreamReader(new MemoryStream(buffer), Encoding.UTF8);

                this.treeView1.Nodes.Clear();
                TreeViewSerializer serializer = new TreeViewSerializer();
                serializer.DeserializeTreeView(this.treeView1, reader);
            }
            //==================================================================================

            foreach (TreeNode node in treeView1.Nodes)
            {
                {
                    //if (node.Nodes.Count > 0)
                    //{
                    //    node.Text = "[" + node.Nodes.Count + "]" + node.Text;
                    //    add_nodes_count(node);
                    //}
                }
                Set_SelectedImageIndex_in_treeView_like_IamgeIndex(node);
            }

            //==========================[ContextMenu for treeView]=============================
            ContextMenu cm = new ContextMenu();
            cm.MenuItems.Add(Lang[17]);
            cm.MenuItems.Add(Lang[10]);
            cm.MenuItems.Add(Lang[11]);
            cm.MenuItems.Add(Lang[12]);
            cm.MenuItems.Add(Lang[13]);
            cm.MenuItems[0].Click += new EventHandler(new_connection_window_showing);
            cm.MenuItems[1].Click += new EventHandler(Add_MenuItem_click);
            cm.MenuItems[2].Click += new EventHandler(rename_node_window_showing);
            cm.MenuItems[3].Click += new EventHandler(Delete_all_MenuItem_click);
            cm.MenuItems[4].Click += new EventHandler(Delete_MenuItem_click);
            treeView1.ContextMenu = cm;
            //==================================================================================

            panel3.VerticalScroll.Maximum = 0;

            conWindowAnimation = new Thread(new ThreadStart(conWinAnimation));

            //Send_message_Button.BackgroundImage = Image.FromFile(@"./images/button_send.png");
            panel_home.BackgroundImage = Image.FromFile(@"./images/home_btn_press.png");
            panel_settings.BackgroundImage = Image.FromFile(@"./images/settings_btn.png");

            //==========================[Font color and backgr color]===========================
            {
                this.BackColor = FormBackColor;
                this.ForeColor = CustomForeColor;

                textBox1.Font = new Font("Aire Exterior", 12);
                textBox1.BackColor = CustomBackColor;

                toolbox_panel.BackColor = CustomBackColor;
                panel_settings.BackColor = CustomBackColor;
                panel_home.BackColor = CustomBackColor;

                panel1.BackColor = CustomBackColor;

                panel3.BackColor = FormBackColor;

                panel4.BackColor = CustomBackColor;
                panel5.BackColor = CustomBackColor;
                panel6.BackColor = CustomBackColor;
                treeView1.BackColor = CustomBackColor;
            }
            //==================================================================================

            dataGridView1.BackgroundColor = CustomBackColor;
            dataGridView1.Font = new Font("Aire Exterior", 12);
            dataGridView1.CellBorderStyle = DataGridViewCellBorderStyle.None;
            dataGridView1.DefaultCellStyle.BackColor = CustomBackColor;
            dataGridView1.DefaultCellStyle.Font = new Font("Aire Exterior", 12);
            dataGridView1.DefaultCellStyle.WrapMode = DataGridViewTriState.True;

        }
        //=========================[MainForm moove and resize]==============================
        private const int cGrip = 16;      // Grip size
        protected override void OnPaint(PaintEventArgs e)
        {
            Rectangle rc = new Rectangle(this.ClientSize.Width - cGrip, this.ClientSize.Height - cGrip, cGrip, cGrip);
            ControlPaint.DrawSizeGrip(e.Graphics, CustomBackColor, rc);
            //rc = new Rectangle(0, 0, this.ClientSize.Width, cCaption);
            //e.Graphics.FillRectangle(Brushes.Black, rc);
        }
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x84)
            {  // Trap WM_NCHITTEST
                Point pos = new Point(m.LParam.ToInt32());
                pos = this.PointToClient(pos);
                if (pos.Y < Height - 15)
                {
                    m.Result = (IntPtr)2;  // HTCAPTION
                    return;
                }
                if (pos.X >= this.ClientSize.Width - cGrip && pos.Y >= this.ClientSize.Height - cGrip)
                {
                    m.Result = (IntPtr)17; // HTBOTTOMRIGHT
                    return;
                }
            }
            base.WndProc(ref m);
        }
        private void Main_Resize(object sender, EventArgs e)
        {
            int top_margin = 45;
            int component_margin = 15;
            int bottom_margin = 35;
            int summary_margin = component_margin;

            toolbox_panel.Size = new Size(50, Height - bottom_margin - component_margin);
            toolbox_panel.Location = new System.Drawing.Point(component_margin, top_margin);

            panel_home.Location = new System.Drawing.Point(Convert.ToInt32((toolbox_panel.Width - panel_home.Width) / 2), 10);
            panel_settings.Location = new Point(Convert.ToInt32((toolbox_panel.Width - panel_home.Width) / 2),
                Convert.ToInt32(toolbox_panel.Height - panel_settings.Height - 10));

            summary_margin += toolbox_panel.Size.Width + component_margin;

            treeView1.Size = new Size(Convert.ToInt32(Width * 0.25), Height - bottom_margin - component_margin - 10);
            treeView1.Location = new System.Drawing.Point(summary_margin, top_margin + 10);

            panel1.Size = new Size(Convert.ToInt32(Width * 0.25), Height - bottom_margin - component_margin);
            panel1.Location = new System.Drawing.Point(summary_margin, top_margin);

            textBox3.Size = new Size(panel1.Width - 10, 27);
            textBox2.Size = new Size(panel1.Width - 10, 27);

            button1.Size = new Size(Convert.ToInt32(panel1.Width / 2) - 10, 27);
            button1.Location = new Point(2, 57);

            button2.Size = new Size(Convert.ToInt32(panel1.Width / 2) - 10, 27);
            button2.Location = new Point(button1.Width + 12, 57);

            Node_add_Button.Size = new Size(Convert.ToInt32(panel1.Width / 2) - 10, 27);
            Node_add_Button.Location = new Point(button1.Width + 12, 57);

            summary_margin += panel1.Width + component_margin;

            panel3.Size = new Size(Width - summary_margin - component_margin, Height + 500);
            panel3.Location = new System.Drawing.Point(summary_margin, top_margin);
            panel3.HorizontalScroll.Value = 0;

            panel4.Size = new Size(Width - summary_margin - component_margin, Height - bottom_margin - component_margin);
            panel4.Location = new System.Drawing.Point(0, 0);


            if (textBox1.Size.Height >= 50)
            {
                dataGridView1.Size = new Size(Width - summary_margin - component_margin, panel4.Height - 50);
                dataGridView1.Location = new System.Drawing.Point(0, 0);

                textBox1.Size = new Size(Width - summary_margin - component_margin, 50);
                textBox1.Location = new System.Drawing.Point(0, panel4.Height - 50);
            }
            else
            {
                dataGridView1.Size = new Size(Width - summary_margin - component_margin, Convert.ToInt32(panel4.Height * 0.9));
                dataGridView1.Location = new System.Drawing.Point(0, 0);

                textBox1.Size = new Size(Width - summary_margin - component_margin, Convert.ToInt32(panel4.Height * 0.1));
                textBox1.Location = new System.Drawing.Point(0, Convert.ToInt32(panel4.Height * 0.9));
            }

            panel5.Size = new Size(Width - summary_margin - component_margin, Height - bottom_margin - component_margin);
            panel5.Location = new System.Drawing.Point(panel4.Width, 0);

            panel6.Size = new Size(Width - summary_margin - component_margin, Height - bottom_margin - component_margin);
            panel6.Location = new System.Drawing.Point(panel4.Width * 2, 0);

            panel7.Location = new System.Drawing.Point(Width - 31, 2);
        }
        private void Main_ResizeEnd(object sender, EventArgs e)
        {
            panel_home.BackgroundImage = Image.FromFile(@"./images/home_btn_press.png");
            panel_settings.BackgroundImage = Image.FromFile(@"./images/settings_btn.png");
        }
        //==================================================================================
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            serialize_tree_view();
            //stop receiving 
            receive.Abort();
            receive.Join(500);
        }
        private void nullToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //copy ip in buffer
            Clipboard.SetText(nullLocalIPToolStripMenuItem.Text);
        }
        private void nullIPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //copy ip in buffer
            Clipboard.SetText(nullIPToolStripMenuItem.Text);
        }
        private void Display(string message)
        {
            //add messages in richtextbox by delegate
            if (message.Split('`')[0] == "0")
            {
                string ip = message.Split('`')[1];
                //richTextBox1.Text += "[" + ip + "] " + Form1.Lang[6] + "\n";

                if (!check_on_match_in_treeView(ip))
                    TreeViewNodes(ip);
            }
            else if (message.Split('`')[0] == "2")
            {
                Action act = new Action(() =>
                {
                    string address = message.Split('`')[1];
                    string data = message.Split('`')[2];
                    //richTextBox1.Text += "[" + address + "] " + data + "\n";

                    if (!sqlite.Check_table_exist(address))
                    {
                        sqlite.ExecuteRequest("CREATE TABLE '" + address + "'(" +
                                                                        "id INTEGER PRIMARY KEY AUTOINCREMENT, " +
                                                                        "sender VARCHAR(30), " +
                                                                        "date VARCHAR(30), " +
                                                                        "data TEXT)");
                    }

                    sqlite.ExecuteRequest("INSERT INTO '" + address + "' VALUES (NULL, '" + address + "', '" + DateTime.Now + "','" + data + "')");
                    if (label2.Text == address)
                    {
                        DataTable dt = (DataTable)dataGridView1.DataSource;
                        dt.Rows.Add();
                        dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[1].Value = "" + DateTime.Now;
                        dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[2].Value = data;
                        dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[2].Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
                        dataGridView1.FirstDisplayedScrollingRowIndex = dataGridView1.RowCount - 1;
                    }
                });

                if (InvokeRequired)
                    this.Invoke(act);
                else
                    act();
            }
            else
            {
                if (InvokeRequired)
                    this.Invoke(new Action(() => richTextBox1.Text += message));
                else
                    richTextBox1.Text += message;
            }
        }
        //==================================[TextBoxes]=====================================
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (textBox1.Text != "")
                if (e.KeyChar == (char)13)
                {
                    string text = "";
                    textBox1.Text = textBox1.Text.Trim();
                    string message = textBox1.Text;
                    textBox1.Text = "";
                    //Отправка сообщения через кнопку
                    //Send message by button
                    decimal d = Convert.ToDecimal(message.Length) / 100;
                    if (message != "")
                    {
                        for (int i = 0; i < Math.Ceiling(d); i++)
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

                            try
                            {
                                Connection.Connect();
                                Send.send(info.ConSocket, info.MyIP + ":" + info.MyPort + "`" + text, "2");
                                //richTextBox1.Text += "[ME] " + text + "\n";
                                sqlite.ExecuteRequest("INSERT INTO '" + label2.Text + "' VALUES (NULL, NULL, '" + DateTime.Now + "','" + text + "')");

                                DataTable dt = (DataTable)dataGridView1.DataSource;
                                dt.Rows.Add();
                                dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[1].Value = "" + DateTime.Now;
                                dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[2].Value = text;
                                dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[2].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                                dataGridView1.FirstDisplayedScrollingRowIndex = dataGridView1.RowCount - 1;
                            }
                            catch (Exception ex)
                            {
                                if (treeView1.SelectedNode == null)
                                {
                                    label2.Text = "NO CONNECTION";
                                    MessageBox.Show(ex.ToString());
                                }
                                else if (treeView1.SelectedNode.ImageIndex != 3)
                                {
                                    label2.Text = "NO CONNECTION";
                                    MessageBox.Show(ex.ToString());
                                }
                                else
                                    MessageBox.Show(ex.ToString());
                            }
                            info.ConSocket.Close();
                        }
                    }
                    e.Handled = true;
                }
        }
        private void textBox1_Enter(object sender, EventArgs e)
        {
            textBox1.BorderStyle = BorderStyle.FixedSingle;
        }
        private void textBox1_Leave(object sender, EventArgs e)
        {
            textBox1.BorderStyle = BorderStyle.None;
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            /*Сделай тут типа чтобы в текстбоксе текст 
            шел столбцом а не в одну строку пожалуйста*/

            //Сам делай, заебал 
        }
        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            //Add new address in list (treeView1) by Enter
            if (e.KeyChar == (char)13)
            {
                Node_add_Button_Click(new object(), new EventArgs());
            }
        }
        private void textBox2_Enter(object sender, EventArgs e)
        {
            if (textBox2.Text == Lang[15])
                textBox2.Text = "";
            textBox2.ForeColor = Color.FromArgb(120, 198, 200);
        }
        private void textBox2_Leave(object sender, EventArgs e)
        {
            if (textBox2.Text == "")
            {
                textBox2.Text = Lang[15];
                textBox2.ForeColor = Color.Silver;
            }
        }
        private void textBox3_Enter(object sender, EventArgs e)
        {
            if (textBox3.Text == Lang[16])
                textBox3.Text = "";
            textBox3.ForeColor = Color.FromArgb(120, 198, 200);
        }
        private void textBox3_Leave(object sender, EventArgs e)
        {
            if (textBox3.Text == "")
            {
                textBox3.Text = Lang[16];
                textBox3.ForeColor = Color.Silver;
            }
        }
        private void richTextBox1_Enter(object sender, EventArgs e)
        {
            textBox1.Focus();
        }
        //===================================[Buttons]======================================
        private void button1_Click(object sender, EventArgs e)
        {
            if (!conWindowAnimation.IsAlive)
            {
                if (panel1.Height != 11)
                {
                    conWindowAnimation = new Thread(new ThreadStart(conWinAnimation));
                    conWindowAnimation.Start();
                }
            }
        }
        private void Node_add_Button_Click(object sender, EventArgs e)
        {
            //Add new address in list (treeView1) by button
            if (info.NodeIP == null)
                method();
            else if (info.NodeIP.Address.ToString() != textBox2.Text)
                method();

            //check on not unique ip addresses
            void method()
            {
                try
                {
                    info.NodeIP = IPAddress.Parse(textBox2.Text.Split(':')[0]);
                    info.NodePort = Int32.Parse(textBox2.Text.Split(':')[1]);

                    try
                    {
                        //if (info.ConSocket.Connected)
                        //{
                        //    info.ConSocket =
                        //        new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        //    label2.Text = Lang[1];
                        //}

                        if (!sqlite.Check_table_exist(textBox2.Text))
                        {
                            sqlite.ExecuteRequest("CREATE TABLE '" + textBox2.Text + "'(" +
                                                                            "id INTEGER PRIMARY KEY AUTOINCREMENT, " +
                                                                            "sender VARCHAR(30), " +
                                                                            "date VARCHAR(30), " +
                                                                            "data TEXT)");
                        }

                        Connection.Connect();

                        if (check_on_match_in_treeView(textBox2.Text))
                        {
                            label2.Text = textBox2.Text;

                            textBox2.Text = Lang[15];
                            textBox2.ForeColor = Color.Silver;
                            textBox3.Text = Lang[16];
                            textBox3.ForeColor = Color.Silver;

                            return;
                        }

                        string name = "";
                        if (textBox3.Text != Lang[16] && textBox3.Text != "")
                        {
                            name = textBox3.Text;
                        }
                        else
                            name = textBox2.Text;

                        TreeNode node = new TreeNode(name, 2, 2);
                        node.Tag = textBox2.Text;
                        treeView1.Nodes.Add(node);

                        label2.Text = textBox2.Text;

                        textBox2.Text = Lang[15];
                        textBox2.ForeColor = Color.Silver;
                        textBox3.Text = Lang[16];
                        textBox3.ForeColor = Color.Silver;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString() + "\n\n" + ex.Message);
                    }
                }
                catch
                {
                    ;
                }
            }
        }
        //==========================[TreeView Drag&Drop Events]=============================
        private void treeView1_ItemDrag(object sender, ItemDragEventArgs e)
        {
            // Move the dragged node when the left mouse button is used.  
            if (e.Button == MouseButtons.Left)
            {
                DoDragDrop(e.Item, DragDropEffects.Move);
            }

            //// Copy the dragged node when the right mouse button is used.  
            //else if (e.Button == MouseButtons.Right)
            //{
            //    DoDragDrop(e.Item, DragDropEffects.Copy);
            //}
        }
        private void treeView1_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = e.AllowedEffect;
        }
        private void treeView1_DragOver(object sender, DragEventArgs e)
        {
            // Retrieve the client coordinates of the mouse position.  
            Point targetPoint = treeView1.PointToClient(new Point(e.X, e.Y));

            // Select the node at the mouse position.  
            treeView1.SelectedNode = treeView1.GetNodeAt(targetPoint);
        }
        private bool ContainsNode(TreeNode node1, TreeNode node2)
        {
            // Check the parent node of the second node.  
            if (node2 == null) return false;
            if (node2.Parent == null) return false;
            if (node2.Parent.Equals(node1)) return true;

            // If the parent node is not null or equal to the first node,   
            // call the ContainsNode method recursively using the parent of   
            // the second node.  
            return ContainsNode(node1, node2.Parent);
        }
        private void treeView1_DragDrop(object sender, DragEventArgs e)
        {
            // Retrieve the client coordinates of the drop location.  
            Point targetPoint = treeView1.PointToClient(new Point(e.X, e.Y));

            // Retrieve the node at the drop location.  
            TreeNode targetNode = treeView1.GetNodeAt(targetPoint);

            // Retrieve the node that was dragged.  
            TreeNode draggedNode = (TreeNode)e.Data.GetData(typeof(TreeNode));

            // Confirm that the node at the drop location is not   
            // the dragged node or a descendant of the dragged node.  
            if (!draggedNode.Equals(targetNode) && !ContainsNode(draggedNode, targetNode))
            {
                // If it is a move operation, remove the node from its current   
                // location and add it to the node at the drop location.  
                if (e.Effect == DragDropEffects.Move)
                {
                    draggedNode.Remove();
                    if (targetNode == null)
                        treeView1.Nodes.Add(draggedNode);
                    else
                        targetNode.Nodes.Add(draggedNode);
                }

                // If it is a copy operation, clone the dragged node   
                // and add it to the node at the drop location.  
                else if (e.Effect == DragDropEffects.Copy)
                {
                    if (targetNode == null)
                        treeView1.Nodes.Add((TreeNode)draggedNode.Clone());
                    else
                        targetNode.Nodes.Add((TreeNode)draggedNode.Clone());
                }

                // Expand the node at the location   
                // to show the dropped node.
                if (targetNode != null)
                    targetNode.Expand();
            }
        }
        //==========================[TreeView ContextMenu Events]===========================
        //===============================[Animation]=========================
        TreeNode renaming_node;
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

            if (treeView1.Location.Y == 55)
            {
                for (i = 11; i <= 90; i++)
                {
                    if (InvokeRequired)
                        this.Invoke(act_show);
                    else
                        act_show();
                    Thread.Sleep(10);
                }
            }
            else
            {
                for (i = 90; i > 10; i--)
                {
                    if (InvokeRequired)
                        this.Invoke(act_hide);
                    else
                        act_hide();
                    Thread.Sleep(10);
                }
            }
            conWindowAnimation.Abort();

        }
        private void new_connection_window_showing(object sender, EventArgs e)
        {
            if (!conWindowAnimation.IsAlive)
            {
                if (treeView1.Location.X == 55)
                {
                    Node_add_Button.Visible = true;
                    textBox2.Enabled = true;
                    textBox2.Visible = true;
                    button2.Visible = false;
                    textBox2.Text = Lang[15];
                    textBox2.ForeColor = Color.Silver;
                    textBox3.Text = Lang[16];
                    textBox3.ForeColor = Color.Silver;

                    conWindowAnimation = new Thread(new ThreadStart(conWinAnimation));
                    conWindowAnimation.Start();
                }
                else
                {
                    conWindowAnimation = new Thread(new ThreadStart(conWinAnimation));
                    conWindowAnimation.Start();
                }
            }
        }
        private void rename_node_window_showing(object sender, EventArgs e)
        {
            if (treeView1.Location.Y == 55)
            {
                renaming_node = treeView1.SelectedNode;
                textBox3.Text = renaming_node.Text;
                textBox3.ForeColor = Color.FromArgb(120, 198, 200);

                if (renaming_node.ImageIndex == 2)
                {
                    textBox2.Text = renaming_node.Tag.ToString();
                    textBox2.Visible = true;
                    textBox2.Enabled = false;
                }
                else if (renaming_node.ImageIndex == 1)
                {
                    textBox2.Visible = false;
                    textBox2.Enabled = true;
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
        //===================================================================
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
            if (textBox3.Text != "")
            {
                renaming_node.Text = textBox3.Text;

                conWindowAnimation = new Thread(new ThreadStart(conWinAnimation));
                conWindowAnimation.Start();

                textBox2.Text = Lang[15];
                textBox2.ForeColor = Color.Silver;
                textBox3.Text = Lang[16];
                textBox3.ForeColor = Color.Silver;
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

            if (this.InvokeRequired) // Вызов метода из другого потока
            {
                TreeViewCallBack callBack = new TreeViewCallBack(TreeViewNodes);
                this.Invoke(callBack, new object[] { ip });
            }
            else // Вызов метода из текущего потока
            {
                TreeNode node = new TreeNode(ip, 2, 2);
                node.Tag = ip;
                treeView1.Nodes.Add(node);
            }
        }
        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (e.Node.ImageIndex == 1)
                {
                    if (e.Button == MouseButtons.Left)
                        if (e.Node.IsExpanded)
                            e.Node.Collapse();
                        else
                            e.Node.Expand();
                }
            }

            if (e.Button == MouseButtons.Right)
                treeView1.SelectedNode = e.Node;
        }
        private void treeView1_KeyPress(object sender, KeyPressEventArgs e)
        {
            //Connection by Enter on node

            if (e.KeyChar == (char)13)
            {
                if (treeView1.SelectedNode != null)
                    if (treeView1.SelectedNode.ImageIndex.ToString() == "2")
                    {
                        info.ConSocket =
                                    new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        info.NodeIP =
                            IPAddress.Parse(treeView1.SelectedNode.Tag
                                .ToString().Split(':')[0]);
                        info.NodePort =
                            Int32.Parse(treeView1.SelectedNode.Tag
                                .ToString().Split(':')[1]);

                        Connection.Connect();

                        label2.Text = info.NodeIP + ":" + info.NodePort;
                    }
            }
        }
        private void treeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            //connection to node by click on node in treeview

            void method()
            {
                if (e.Node.ImageIndex == 2)
                {
                    info.NodeIP = IPAddress.Parse(e.Node.Tag.ToString().Split(':')[0]);
                    info.NodePort = Int32.Parse(e.Node.Tag.ToString().Split(':')[1]);

                    try
                    {
                        Connection.Connect();

                        label2.Text = e.Node.Tag.ToString();

                        if (!sqlite.Check_table_exist(e.Node.Tag.ToString()))
                        {
                            sqlite.ExecuteRequest("CREATE TABLE '" + e.Node.Tag.ToString() + "'(" +
                                                                            "id INTEGER PRIMARY KEY AUTOINCREMENT, " +
                                                                            "sender VARCHAR(30), " +
                                                                            "date VARCHAR(30), " +
                                                                            "data TEXT)");
                        }
                        DataTable dt = (DataTable)dataGridView1.DataSource;
                        if (dt != null)
                            dt.Rows.Clear();

                        sqlite.Select("SELECT sender, date, data FROM '" + e.Node.Tag.ToString() + "' WHERE id IN " +
                            "(SELECT  id " +
                            "FROM '" + e.Node.Tag.ToString() + "' " +
                            "ORDER BY id DESC LIMIT 10) " +
                            "ORDER BY id ASC", dataGridView1);

                        dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
                        dataGridView1.Columns[1].DefaultCellStyle.ForeColor = Color.Silver;
                        dataGridView1.Columns[0].Visible = false;

                        foreach (DataGridViewRow row in dataGridView1.Rows)
                        {
                            if (row.Cells[0].Value.ToString() == "")
                                row.Cells[2].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            else
                                row.Cells[2].Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
                        }

                        dataGridView1.Columns[1].Width = (dataGridView1.Width * 20) / 100;
                        dataGridView1.Columns[2].Width = (dataGridView1.Width * 80) / 100;

                        if (dataGridView1.RowCount > 2)
                            dataGridView1.FirstDisplayedScrollingRowIndex = dataGridView1.RowCount - 1;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                }
            }

            if (e.Button == MouseButtons.Left)
            {
                if (treeView1.SelectedNode != null && e.Node.ImageIndex == 2)
                {
                    try
                    {
                        if (info.NodeIP == null)
                            method();
                        else if (info.NodeIP.ToString() != textBox2.Text)
                            method();
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
                    if (node.Nodes[i].Tag == null)
                    {
                        if (check_node(node.Nodes[i]))
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
                if (node.Tag == null)
                {
                    if (check_node(node))
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

            SQLite sqlite = new SQLite();
            sqlite.Connect(@"./DataBases/DataBase.db3");

            sqlite.Insert_BLOB("UPDATE treeViews SET tree = @file");
        }
        //===========================[Slider and animation]=================================
        int position = 0;
        private void timer1_Tick(object sender, EventArgs e)
        {
            var scroll = panel3.HorizontalScroll;

            if (position != scroll.Value)
                if (scroll.Value > position)
                {
                    if (scroll.Value <= position + 100)
                        scroll.Value = position;
                    else
                        scroll.Value -= 100;
                }
                else
                {
                    if (scroll.Value >= position - 100)
                        scroll.Value = position;
                    else
                        scroll.Value += 100;
                }
            else
                timer1.Enabled = false;
        }
        private void panel_home_Click(object sender, EventArgs e)
        {
            if (timer1.Enabled == false)
            {
                panel_home.BackgroundImage = Image.FromFile(@"./images/home_btn_press.png");
                panel_settings.BackgroundImage = Image.FromFile(@"./images/settings_btn.png");
                position = 0;
                timer1.Enabled = true;
            }
        }
        private void button5_Click(object sender, EventArgs e)
        {
            if (timer1.Enabled == false)
            {
                panel_settings.BackgroundImage = Image.FromFile(@"./images/settings_btn.png");
                panel_home.BackgroundImage = Image.FromFile(@"./images/home_btn.png");
                position = panel5.Width;
                timer1.Enabled = true;
            }
        }
        private void panel_settings_Click(object sender, EventArgs e)
        {
            if (timer1.Enabled == false)
            {
                panel_home.BackgroundImage = Image.FromFile(@"./images/home_btn.png");
                panel_settings.BackgroundImage = Image.FromFile(@"./images/settings_btn_press.png");
                position = panel6.Width * 2;
                timer1.Enabled = true;
            }
        }
        private void Main_SizeChanged(object sender, EventArgs e)
        {
            //panel_settings.Location = new System.Drawing.Point(panel_home.Location.X, panel2.Height - 44);
        }
        //===============================[Form moving]======================================
        private void panel7_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void panel7_MouseHover(object sender, EventArgs e)
        {
            panel7.BackColor = Color.Red;
        }
        private void panel7_MouseLeave(object sender, EventArgs e)
        {
            panel7.BackColor = Color.Blue;
        }
        private void resize_panel_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
        }
        //==================================================================================

        //private byte[] GetBinaryFile(string filename)
        //{
        //    byte[] bytes;
        //    using (FileStream file = new FileStream(filename, FileMode.Open, FileAccess.Read))
        //    {
        //        bytes = new byte[file.Length];
        //        file.Read(bytes, 0, (int)file.Length);
        //        file.Close();
        //    }
        //    return bytes;
        //}
        //==================================================================================


        //==================================================================================


    }
}

//system codes 
//type of sended data
//0 - подключение (connection)
//2 - отправка сообщения (message)
