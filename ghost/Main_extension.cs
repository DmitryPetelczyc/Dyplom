using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace ghost
{
    public partial class Main : Form
    {
        private void set_treeView_settings()
        {
            string path = "./images/ico/treeView/";

            ImageList imglist_treeView = new ImageList
            {
                ImageSize = new Size(21, 21),
                ColorDepth = ColorDepth.Depth32Bit
            };

            imglist_treeView.Images.Add("1", Image.FromFile(path + "void.png"));
            imglist_treeView.Images.Add("2", Image.FromFile(path + "folder.png"));
            imglist_treeView.Images.Add("3", Image.FromFile(path + "laptop.png"));
            imglist_treeView.Images.Add("4", Image.FromFile(path + "user.png"));
            imglist_treeView.Images.Add("5", Image.FromFile(path + "group.png"));

            imglist[0] = Image.FromFile(@"./images/home_btn.png");
            imglist[1] = Image.FromFile(@"./images/home_btn_press.png");
            imglist[2] = Image.FromFile(@"./images/settings_btn.png");
            imglist[3] = Image.FromFile(@"./images/settings_btn_press.png");
            imglist[4] = Image.FromFile(@"./images/autosend.png");
            imglist[5] = Image.FromFile(@"./images/autosend_press.png");

            treeView1.ImageList = imglist_treeView;
            treeView1.BorderStyle = BorderStyle.None;
            treeView1.LineColor = Color.Gray;
            treeView1.ShowLines = false;
            treeView1.ShowPlusMinus = false;
            treeView1.BackColor = BackColor;
            treeView1.AllowDrop = true;
            treeView1.BackColor = Color.Black;
            treeView1.Font = new Font("Aire Exterior", 12);

            treeView1.ItemDrag += new ItemDragEventHandler(treeView1_ItemDrag);
            treeView1.DragEnter += new DragEventHandler(treeView1_DragEnter);
            treeView1.DragOver += new DragEventHandler(treeView1_DragOver);
            treeView1.DragDrop += new DragEventHandler(treeView1_DragDrop);
        }
        private void database_connection()
        {
            //Creating new database if db not exist
            if (!File.Exists(@"./DataBases/DataBase.db3"))
            {
                DataBase DB = new DataBase();
                DB.create_new_data_base();
            }

            sqlite.Connect();

            DataSet ds = sqlite.Select_DataSet("SELECT * FROM `treeViews`"); //  WHERE name LIKE 'test'
            byte[] buffer;
            if (ds.Tables[0].Rows[ds.Tables[0].Rows.Count - 1].ItemArray[2] != null)
            {
                buffer = (byte[])ds.Tables[0].Rows[ds.Tables[0].Rows.Count - 1].ItemArray[2];

                var reader = new StreamReader(new MemoryStream(buffer), Encoding.UTF8);

                this.treeView1.Nodes.Clear();
                TreeViewSerializer serializer = new TreeViewSerializer();
                serializer.DeserializeTreeView(this.treeView1, reader);
            }
        }
        private void Set_SelectedImageIndex_in_treeView_like_IamgeIndex(TreeNode node)
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
        private void set_font_and_background()
        {
            this.BackColor = FormBackColor;
            this.ForeColor = CustomForeColor;

            send_message_text_box.Font = new Font("Aire Exterior", 12);
            send_message_text_box.BackColor = CustomBackColor;
            send_message_text_box.BorderStyle = BorderStyle.FixedSingle;

            autosend_textbox.Font = new Font("Aire Exterior", 12);
            autosend_textbox.BackColor = CustomBackColor;
            autosend_textbox.BorderStyle = BorderStyle.FixedSingle;

            toolbox_panel.BackColor = CustomBackColor;
            panel_settings.BackColor = CustomBackColor;
            panel_home.BackColor = CustomBackColor;
            panel_autosend.BackColor = CustomBackColor;

            panel1.BackColor = CustomBackColor;

            panel3.SuspendLayout();
            panel3.BackColor = FormBackColor;
            panel3.VerticalScroll.Maximum = 10000;
            panel3.VerticalScroll.Minimum = 0;

            panel4.BackColor = CustomBackColor;
            panel5.BackColor = CustomBackColor;
            panel6.BackColor = CustomBackColor;
            treeView1.BackColor = CustomBackColor;

            panel_home.BackgroundImage = imglist[1];
            panel_autosend.BackgroundImage = imglist[4];
            panel_settings.BackgroundImage = imglist[2];

            functional_send_panel.BackColor = CustomBackColor;


            sign_message_panel.BackgroundImage = Image.FromFile(@"./images/pencil.png");
            sign_message_panel.BackColor = CustomBackColor;
            sign_message_panel.BackgroundImageLayout = ImageLayout.Stretch;

            // Create the ToolTip and associate with the Form container.
            ToolTip toolTip1 = new ToolTip();

            // Set up the delays for the ToolTip.
            toolTip1.AutoPopDelay = 5000;
            toolTip1.InitialDelay = 500;
            toolTip1.ReshowDelay = 500;
            // Force the ToolTip text to be displayed whether or not the form is active.
            toolTip1.ShowAlways = true;

            // Set up the ToolTip text for the Button and Checkbox.
            toolTip1.SetToolTip(this.sign_message_panel, "Подписать сообщение");

            file_send_panel.BackColor = CustomBackColor;

            panel10.BackColor = CustomBackColor;

            panel11.BackColor = CustomBackColor;

        }
        private void set_dgv_settings()
        {
            message_view_dgv.BackgroundColor = CustomBackColor;
            message_view_dgv.Font = new Font("Aire Exterior", 12);
            message_view_dgv.CellBorderStyle = DataGridViewCellBorderStyle.None;
            message_view_dgv.DefaultCellStyle.BackColor = CustomBackColor;
            message_view_dgv.DefaultCellStyle.Font = new Font("Aire Exterior", 12);
            message_view_dgv.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            message_view_dgv.Columns.Add("id", "");
            message_view_dgv.Columns.Add("sender", "");
            message_view_dgv.Columns.Add("time", "");
            message_view_dgv.Columns.Add("data", "");
            message_view_dgv.Columns.Add("isSigned", "");

            message_view_dgv.Columns["id"].Visible = false;
            message_view_dgv.Columns["sender"].Visible = false;
            message_view_dgv.Columns["isSigned"].Visible = false;

            message_view_dgv.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCellsExceptHeaders;
            message_view_dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            //message_view_dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            message_view_dgv.Columns[2].DefaultCellStyle.ForeColor = Color.Silver;
            message_view_dgv.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            autosend_dgv.BackgroundColor = CustomBackColor;
            autosend_dgv.Font = new Font("Aire Exterior", 12);
            autosend_dgv.CellBorderStyle = DataGridViewCellBorderStyle.None;
            autosend_dgv.DefaultCellStyle.BackColor = CustomBackColor;
            autosend_dgv.DefaultCellStyle.Font = new Font("Aire Exterior", 12);
            autosend_dgv.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            autosend_dgv.MultiSelect = false;
        }
        //===================================================================================
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
        //===================================================================================
        private void Main_ResizeEnd(object sender, EventArgs e)
        {
            panel_home.BackgroundImage = imglist[1];
            panel_autosend.BackgroundImage = imglist[4];
            panel_settings.BackgroundImage = imglist[2];
            panel3.HorizontalScroll.Value = 0;
        }
        private void Main_Move(object sender, EventArgs e)
        {
            panel3.HorizontalScroll.Value = 0;
        }
        private void form_close_panel_MouseHover(object sender, EventArgs e)
        {
            form_close_panel.BackColor = Color.Red;
        }
        private void form_close_panel_MouseLeave(object sender, EventArgs e)
        {
            form_close_panel.BackColor = Color.Blue;
        }
        private void resize_panel_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Maximized;
            panel_home.BackgroundImage = imglist[1];
            panel_autosend.BackgroundImage = imglist[4];
            panel_settings.BackgroundImage = imglist[2];
            panel3.HorizontalScroll.Value = 0;
        }
        private void trey_form_panel_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }
        private void form_close_panel_Click(object sender, EventArgs e)
        {
            Close();
        }
        //==================================[TextBoxes]=====================================
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
            if (address_textbox.Text == Lang[15])
                address_textbox.Text = "";
            address_textbox.ForeColor = Color.Black;//Color.FromArgb(120, 198, 200);
        }
        private void textBox2_Leave(object sender, EventArgs e)
        {
            if (address_textbox.Text == "")
            {
                address_textbox.Text = Lang[15];
                address_textbox.ForeColor = Color.Silver;
            }
        }
        private void textBox3_Enter(object sender, EventArgs e)
        {
            if (global_name_textbox.Text == Lang[16])
                global_name_textbox.Text = "";
            global_name_textbox.ForeColor = Color.Black;//Color.FromArgb(120, 198, 200);
        }
        private void textBox3_Leave(object sender, EventArgs e)
        {
            if (global_name_textbox.Text == "")
            {
                global_name_textbox.Text = Lang[16];
                global_name_textbox.ForeColor = Color.Silver;
            }
        }
        private void open_key_textbox_Enter(object sender, EventArgs e)
        {
            if (open_key_textbox.Text == "Публичный ключ")
                open_key_textbox.Text = "";
            open_key_textbox.ForeColor = Color.Black;//Color.FromArgb(120, 198, 200);
        }
        private void open_key_textbox_Leave(object sender, EventArgs e)
        {
            if (open_key_textbox.Text == "")
            {
                open_key_textbox.Text = "Публичный ключ";
                open_key_textbox.ForeColor = Color.Silver;
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

            if (draggedNode.ImageIndex == 2)
                return;

            // Confirm that the node at the drop location is not   
            // the dragged node or a descendant of the dragged node.  
            if (!draggedNode.Equals(targetNode) && !ContainsNode(draggedNode, targetNode))
            {
                // If it is a move operation, remove the node from its current   
                // location and add it to the node at the drop location.  


                if (e.Effect == DragDropEffects.Move)
                {
                    if (targetNode == null)
                    {
                        draggedNode.Remove();
                        treeView1.Nodes.Add(draggedNode);
                    }
                    else if (targetNode.ImageIndex != 2 &&
                             targetNode.ImageIndex != 3)
                    {
                        draggedNode.Remove();
                        targetNode.Nodes.Add(draggedNode);
                    }
                }

                // If it is a copy operation, clone the dragged node   
                // and add it to the node at the drop location.  
                else if (e.Effect == DragDropEffects.Copy)
                {
                    if (targetNode == null)
                        treeView1.Nodes.Add((TreeNode)draggedNode.Clone());
                    else if (targetNode.ImageIndex != 2 &&
                             targetNode.ImageIndex != 3)
                        targetNode.Nodes.Add((TreeNode)draggedNode.Clone());
                }

                // Expand the node at the location   
                // to show the dropped node.
                if (targetNode != null)
                    targetNode.Expand();
            }
        }
        //===================================================================================
        private void add_user_panel_Click(object sender, EventArgs e)
        {
            win_anim_offset = 0;

            global_name_textbox.ReadOnly = false;
            address_textbox.ReadOnly = false;
            open_key_textbox.ReadOnly = false;

            button1.Location = new Point(2, 85); // 85
            button2.Location = new Point(button1.Width + 12, 85);
            Node_add_Button.Location = new Point(button1.Width + 12, 85);

            new_connection_window_showing(new object(), new EventArgs());
        }
        private void add_folder_panel_Click(object sender, EventArgs e)
        {
            treeView1.SelectedNode = null;
            Add_MenuItem_click(new object(), new EventArgs());
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
                slider_animation_timer.Enabled = false;
        }
        private void panel_home_Click(object sender, EventArgs e)
        {
            if (slider_animation_timer.Enabled == false)
            {
                panel_home.BackgroundImage = imglist[1];
                panel_autosend.BackgroundImage = imglist[4];
                panel_settings.BackgroundImage = imglist[2];
                position = 0;
                current_slider = 0;
                slider_animation_timer.Enabled = true;
            }
        }
        private void autosend_panel_Click(object sender, EventArgs e)
        {
            if (slider_animation_timer.Enabled == false)
            {
                panel_settings.BackgroundImage = imglist[2];
                panel_autosend.BackgroundImage = imglist[5];
                panel_home.BackgroundImage = imglist[0];
                position = panel5.Width;
                current_slider = 1;
                fill_autosend_dgv();
                slider_animation_timer.Enabled = true;
            }
        }
        private void panel_settings_Click(object sender, EventArgs e)
        {
            if (slider_animation_timer.Enabled == false)
            {
                panel_home.BackgroundImage = imglist[0];
                panel_autosend.BackgroundImage = imglist[4];
                panel_settings.BackgroundImage = imglist[3];
                position = panel6.Width * 2;
                current_slider = 2;
                slider_animation_timer.Enabled = true;
            }
        }
        //===================================================================================
        private string get_public_key_by_blobal_name(string global_name)
        {
            DataSet ds = sqlite.Select_DataSet("SELECT open_key FROM Keys WHERE global_name LIKE '" + global_name + "'");
            if (ds.Tables[0].Rows.Count > 0)
                return ds.Tables[0].Rows[0].ItemArray[0].ToString();
            else
                return null;
        }
        private void show_dialog(object sender, EventArgs e)
        {
            TreeNode node = treeView1.SelectedNode;

            current_dialog = node.Tag.ToString();

            if (node.ImageIndex == 4)
            { group = true; send_message_text_box.Enabled = true; } //<==========================================================
            else
                group = false;

            label2.Text = node.Text;

            message_view_dgv.Rows.Clear();

            DataSet ds = sqlite.Select_DataSet("SELECT id, sender, date, data, sign FROM '" + node.Tag.ToString() + "' WHERE id IN " +
                "(SELECT  id " +
                "FROM '" + node.Tag.ToString() + "' " +
                "ORDER BY id DESC LIMIT " + 0 + ", " + 40 + ") " +
                "ORDER BY id DESC");

            //message_offset += 40;
            fill_datagrid(ds);
        }
        private void fill_datagrid(DataSet dt)
        {
            DataTable table = dt.Tables[0];
            DataGridView dgv = message_view_dgv;

            object[] row;

            string date;

            if (message_view_dgv.Rows.Count > 0)
                message_view_dgv.Rows.Remove(message_view_dgv.Rows[0]);

            for (int i = 0; i < table.Rows.Count; i++)
            {
                date = table.Rows[i].ItemArray[2].ToString().Split(' ')[1];
                date = date.Split(':')[0] + ":" + date.Split(':')[1];

                //===========================================================================
                //Clear a row
                row = new object[5];
                //id
                row[0] = table.Rows[i].ItemArray[0].ToString();
                //sender
                row[1] = table.Rows[i].ItemArray[1].ToString();
                //date
                row[2] = date;
                //message
                if (table.Rows[i].ItemArray[4].ToString() == "")
                {
                    row[3] = table.Rows[i].ItemArray[3].ToString();
                    //signed
                    row[4] = "";
                }
                else
                {
                    row[3] = table.Rows[i].ItemArray[3].ToString();
                    row[4] = table.Rows[i].ItemArray[4].ToString();
                }
                //===========================================================================
                //Insert new row in dataGridView
                dgv.Rows.Insert(0, row);
                //===========================================================================
                //if box of sender is empty it means my message. then make him right aligment
                if (dgv.Rows[0].Cells[1].Value.ToString() == "")
                    dgv.Rows[0].Cells[3].Style.Alignment =
                                            DataGridViewContentAlignment.MiddleRight;
                //else left aligment
                else
                    dgv.Rows[0].Cells[3].Style.Alignment =
                                            DataGridViewContentAlignment.MiddleLeft;

                //customize signed message
                if (dgv.Rows[0].Cells[4].Value.ToString() != "")
                {
                    dgv.Rows[0].Cells[3].Style.Font =
                        new Font("Aire Exterior", 12);

                    dgv.Rows[0].Cells[3].Style.ForeColor =
                        Color.FromArgb(120, 198, 200);
                }

                if (i + 1 < table.Rows.Count)
                    if (table.Rows[i].ItemArray[2].ToString().Split(' ')[0]
                        !=
                        table.Rows[i + 1].ItemArray[2].ToString().Split(' ')[0])
                    {
                        row = new object[5];
                        row[3] = table.Rows[i].ItemArray[2].ToString().Split(' ')[0];
                        dgv.Rows.Insert(0, row);
                        dgv.Rows[0].Cells[3].Style.Alignment =
                                                    DataGridViewContentAlignment.MiddleCenter;
                        dgv.Rows[0].Cells[3].Style.ForeColor = Color.Gray;
                    }
            }

            if (message_view_dgv.RowCount > 2)
                message_view_dgv.FirstDisplayedScrollingRowIndex = message_view_dgv.RowCount - 1;

            if (table.Rows.Count > 0)
            {
                row = new object[5];
                row[3] = table.Rows[table.Rows.Count - 1].ItemArray[2].ToString().Split(' ')[0];
                dgv.Rows.Insert(0, row);
                dgv.Rows[0].Cells[3].Style.Alignment =
                                        DataGridViewContentAlignment.MiddleCenter;
                dgv.Rows[0].Cells[3].Style.ForeColor = Color.Gray;
            }
            dgv.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
        }
        //===================================================================================
        //===================================================================================
        //===================================================================================
        //===================================================================================
        //===================================================================================
        //===================================================================================
        //===================================================================================
        //===================================================================================
        //===================================================================================
        //===================================================================================
        //===================================================================================
        //===================================================================================
        //===================================================================================
        //===================================================================================
        //===================================================================================
        //===================================================================================
    }

}
