namespace ghost
{
    partial class Main
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle13 = new System.Windows.Forms.DataGridViewCellStyle();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.infoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.myIPAddressToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.nullLocalIPToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.iPAddressToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.nullIPToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel_home = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel6 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.panel5 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.button5 = new System.Windows.Forms.Button();
            this.panel_settings = new System.Windows.Forms.Panel();
            this.panel7 = new System.Windows.Forms.Panel();
            this.toolbox_panel = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.button2 = new System.Windows.Forms.Button();
            this.Node_add_Button = new System.Windows.Forms.Button();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.resize_panel = new System.Windows.Forms.Panel();
            this.menuStrip1.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel6.SuspendLayout();
            this.panel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.panel5.SuspendLayout();
            this.toolbox_panel.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.SystemColors.WindowText;
            this.menuStrip1.Font = new System.Drawing.Font("Aire Exterior", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.infoToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.menuStrip1.Size = new System.Drawing.Size(976, 28);
            this.menuStrip1.TabIndex = 9;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // infoToolStripMenuItem
            // 
            this.infoToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.myIPAddressToolStripMenuItem,
            this.iPAddressToolStripMenuItem});
            this.infoToolStripMenuItem.ForeColor = System.Drawing.SystemColors.Window;
            this.infoToolStripMenuItem.Name = "infoToolStripMenuItem";
            this.infoToolStripMenuItem.Size = new System.Drawing.Size(50, 23);
            this.infoToolStripMenuItem.Text = "info";
            // 
            // myIPAddressToolStripMenuItem
            // 
            this.myIPAddressToolStripMenuItem.BackColor = System.Drawing.SystemColors.Window;
            this.myIPAddressToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.nullLocalIPToolStripMenuItem});
            this.myIPAddressToolStripMenuItem.Name = "myIPAddressToolStripMenuItem";
            this.myIPAddressToolStripMenuItem.Size = new System.Drawing.Size(187, 26);
            this.myIPAddressToolStripMenuItem.Text = "My IP Address";
            // 
            // nullLocalIPToolStripMenuItem
            // 
            this.nullLocalIPToolStripMenuItem.Name = "nullLocalIPToolStripMenuItem";
            this.nullLocalIPToolStripMenuItem.Size = new System.Drawing.Size(118, 26);
            this.nullLocalIPToolStripMenuItem.Text = "null";
            this.nullLocalIPToolStripMenuItem.Click += new System.EventHandler(this.nullToolStripMenuItem_Click);
            // 
            // iPAddressToolStripMenuItem
            // 
            this.iPAddressToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.nullIPToolStripMenuItem});
            this.iPAddressToolStripMenuItem.Name = "iPAddressToolStripMenuItem";
            this.iPAddressToolStripMenuItem.Size = new System.Drawing.Size(187, 26);
            this.iPAddressToolStripMenuItem.Text = "IP Address";
            // 
            // nullIPToolStripMenuItem
            // 
            this.nullIPToolStripMenuItem.Name = "nullIPToolStripMenuItem";
            this.nullIPToolStripMenuItem.Size = new System.Drawing.Size(118, 26);
            this.nullIPToolStripMenuItem.Text = "null";
            this.nullIPToolStripMenuItem.Click += new System.EventHandler(this.nullIPToolStripMenuItem_Click);
            // 
            // panel_home
            // 
            this.panel_home.BackColor = System.Drawing.SystemColors.HotTrack;
            this.panel_home.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel_home.Location = new System.Drawing.Point(2, 0);
            this.panel_home.Name = "panel_home";
            this.panel_home.Size = new System.Drawing.Size(46, 46);
            this.panel_home.TabIndex = 24;
            this.panel_home.Click += new System.EventHandler(this.panel_home_Click);
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.label2.Location = new System.Drawing.Point(282, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(363, 17);
            this.label2.TabIndex = 5;
            this.label2.Text = "no connection";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label2.Visible = false;
            // 
            // richTextBox1
            // 
            this.richTextBox1.BackColor = System.Drawing.SystemColors.HotTrack;
            this.richTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBox1.Font = new System.Drawing.Font("Aire Exterior", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.richTextBox1.ForeColor = System.Drawing.SystemColors.Window;
            this.richTextBox1.Location = new System.Drawing.Point(778, 122);
            this.richTextBox1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.ReadOnly = true;
            this.richTextBox1.Size = new System.Drawing.Size(173, 107);
            this.richTextBox1.TabIndex = 5;
            this.richTextBox1.Text = "";
            this.richTextBox1.Visible = false;
            this.richTextBox1.Enter += new System.EventHandler(this.richTextBox1_Enter);
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.Red;
            this.panel3.Controls.Add(this.panel6);
            this.panel3.Controls.Add(this.panel4);
            this.panel3.Controls.Add(this.panel5);
            this.panel3.Location = new System.Drawing.Point(282, 61);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(400, 337);
            this.panel3.TabIndex = 17;
            // 
            // panel6
            // 
            this.panel6.BackColor = System.Drawing.Color.Green;
            this.panel6.Controls.Add(this.label3);
            this.panel6.Location = new System.Drawing.Point(800, 0);
            this.panel6.Margin = new System.Windows.Forms.Padding(0);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(400, 240);
            this.panel6.TabIndex = 24;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Aire Exterior", 25.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label3.Location = new System.Drawing.Point(3, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(232, 43);
            this.label3.TabIndex = 1;
            this.label3.Text = "В РАЗРАБОТКЕ";
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.DarkCyan;
            this.panel4.Controls.Add(this.textBox1);
            this.panel4.Controls.Add(this.dataGridView1);
            this.panel4.Location = new System.Drawing.Point(0, 0);
            this.panel4.Margin = new System.Windows.Forms.Padding(0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(400, 240);
            this.panel4.TabIndex = 28;
            // 
            // textBox1
            // 
            this.textBox1.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox1.Font = new System.Drawing.Font("Aire Exterior", 10.8F);
            this.textBox1.ForeColor = System.Drawing.Color.White;
            this.textBox1.Location = new System.Drawing.Point(0, 208);
            this.textBox1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(400, 30);
            this.textBox1.TabIndex = 6;
            this.textBox1.WordWrap = false;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            this.textBox1.Enter += new System.EventHandler(this.textBox1_Enter);
            this.textBox1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox1_KeyPress);
            this.textBox1.Leave += new System.EventHandler(this.textBox1_Leave);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToOrderColumns = true;
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridView1.ColumnHeadersHeight = 29;
            this.dataGridView1.ColumnHeadersVisible = false;
            dataGridViewCellStyle13.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle13.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle13.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle13.ForeColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle13.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle13.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle13.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.DefaultCellStyle = dataGridViewCellStyle13;
            this.dataGridView1.Location = new System.Drawing.Point(0, -5);
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.RowHeadersWidth = 51;
            this.dataGridView1.RowTemplate.Height = 24;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(400, 212);
            this.dataGridView1.TabIndex = 27;
            // 
            // panel5
            // 
            this.panel5.BackColor = System.Drawing.Color.Maroon;
            this.panel5.Controls.Add(this.label1);
            this.panel5.Location = new System.Drawing.Point(400, 0);
            this.panel5.Margin = new System.Windows.Forms.Padding(0);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(400, 240);
            this.panel5.TabIndex = 23;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Aire Exterior", 25.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(232, 43);
            this.label1.TabIndex = 0;
            this.label1.Text = "В РАЗРАБОТКЕ";
            // 
            // timer1
            // 
            this.timer1.Interval = 1;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(6, 72);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(46, 46);
            this.button5.TabIndex = 32;
            this.button5.Text = "button5";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // panel_settings
            // 
            this.panel_settings.BackColor = System.Drawing.SystemColors.HotTrack;
            this.panel_settings.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel_settings.Location = new System.Drawing.Point(2, 209);
            this.panel_settings.Name = "panel_settings";
            this.panel_settings.Size = new System.Drawing.Size(46, 46);
            this.panel_settings.TabIndex = 30;
            this.panel_settings.Click += new System.EventHandler(this.panel_settings_Click);
            // 
            // panel7
            // 
            this.panel7.BackColor = System.Drawing.Color.Blue;
            this.panel7.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel7.Location = new System.Drawing.Point(837, 0);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(31, 23);
            this.panel7.TabIndex = 27;
            this.panel7.Click += new System.EventHandler(this.panel7_Click);
            this.panel7.MouseLeave += new System.EventHandler(this.panel7_MouseLeave);
            this.panel7.MouseHover += new System.EventHandler(this.panel7_MouseHover);
            // 
            // toolbox_panel
            // 
            this.toolbox_panel.BackColor = System.Drawing.Color.Red;
            this.toolbox_panel.Controls.Add(this.panel_settings);
            this.toolbox_panel.Controls.Add(this.button5);
            this.toolbox_panel.Controls.Add(this.panel_home);
            this.toolbox_panel.Location = new System.Drawing.Point(12, 46);
            this.toolbox_panel.Name = "toolbox_panel";
            this.toolbox_panel.Size = new System.Drawing.Size(50, 257);
            this.toolbox_panel.TabIndex = 25;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.HotTrack;
            this.panel1.Controls.Add(this.button2);
            this.panel1.Controls.Add(this.Node_add_Button);
            this.panel1.Controls.Add(this.textBox3);
            this.panel1.Controls.Add(this.textBox2);
            this.panel1.Controls.Add(this.button1);
            this.panel1.Location = new System.Drawing.Point(67, 46);
            this.panel1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(206, 110);
            this.panel1.TabIndex = 29;
            // 
            // button2
            // 
            this.button2.Font = new System.Drawing.Font("Aire Exterior", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.button2.Location = new System.Drawing.Point(108, 72);
            this.button2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(95, 27);
            this.button2.TabIndex = 25;
            this.button2.Text = "rename";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Visible = false;
            this.button2.Click += new System.EventHandler(this.rename_node_button_click);
            // 
            // Node_add_Button
            // 
            this.Node_add_Button.Location = new System.Drawing.Point(108, 73);
            this.Node_add_Button.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Node_add_Button.Name = "Node_add_Button";
            this.Node_add_Button.Size = new System.Drawing.Size(95, 27);
            this.Node_add_Button.TabIndex = 22;
            this.Node_add_Button.Text = "----------------claim";
            this.Node_add_Button.UseVisualStyleBackColor = true;
            this.Node_add_Button.Click += new System.EventHandler(this.Node_add_Button_Click);
            // 
            // textBox3
            // 
            this.textBox3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox3.Font = new System.Drawing.Font("Aire Exterior", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textBox3.Location = new System.Drawing.Point(3, 12);
            this.textBox3.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(200, 27);
            this.textBox3.TabIndex = 23;
            this.textBox3.Enter += new System.EventHandler(this.textBox3_Enter);
            this.textBox3.Leave += new System.EventHandler(this.textBox3_Leave);
            // 
            // textBox2
            // 
            this.textBox2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox2.Font = new System.Drawing.Font("Aire Exterior", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textBox2.Location = new System.Drawing.Point(3, 43);
            this.textBox2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(200, 27);
            this.textBox2.TabIndex = 21;
            this.textBox2.Enter += new System.EventHandler(this.textBox2_Enter);
            this.textBox2.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox2_KeyPress);
            this.textBox2.Leave += new System.EventHandler(this.textBox2_Leave);
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Aire Exterior", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.button1.Location = new System.Drawing.Point(3, 72);
            this.button1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(95, 27);
            this.button1.TabIndex = 24;
            this.button1.Text = "close";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // treeView1
            // 
            this.treeView1.BackColor = System.Drawing.SystemColors.MenuBar;
            this.treeView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.treeView1.Font = new System.Drawing.Font("Aire Exterior", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.treeView1.ForeColor = System.Drawing.SystemColors.Window;
            this.treeView1.Location = new System.Drawing.Point(67, 150);
            this.treeView1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(206, 153);
            this.treeView1.TabIndex = 28;
            this.treeView1.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView1_NodeMouseClick);
            this.treeView1.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView1_NodeMouseDoubleClick);
            this.treeView1.DragDrop += new System.Windows.Forms.DragEventHandler(this.treeView1_DragDrop);
            this.treeView1.DragEnter += new System.Windows.Forms.DragEventHandler(this.treeView1_DragEnter);
            this.treeView1.DragOver += new System.Windows.Forms.DragEventHandler(this.treeView1_DragOver);
            this.treeView1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.treeView1_KeyPress);
            // 
            // resize_panel
            // 
            this.resize_panel.BackColor = System.Drawing.Color.Blue;
            this.resize_panel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.resize_panel.Location = new System.Drawing.Point(104, 4);
            this.resize_panel.Name = "resize_panel";
            this.resize_panel.Size = new System.Drawing.Size(31, 23);
            this.resize_panel.TabIndex = 30;
            this.resize_panel.Click += new System.EventHandler(this.resize_panel_Click);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.SystemColors.WindowText;
            this.ClientSize = new System.Drawing.Size(976, 566);
            this.Controls.Add(this.resize_panel);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.treeView1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel7);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.toolbox_panel);
            this.Controls.Add(this.menuStrip1);
            this.ForeColor = System.Drawing.SystemColors.Window;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(560, 320);
            this.Name = "Main";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResizeEnd += new System.EventHandler(this.Main_ResizeEnd);
            this.SizeChanged += new System.EventHandler(this.Main_SizeChanged);
            this.Resize += new System.EventHandler(this.Main_Resize);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel6.ResumeLayout(false);
            this.panel6.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            this.toolbox_panel.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem infoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem myIPAddressToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem nullLocalIPToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem iPAddressToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem nullIPToolStripMenuItem;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Panel panel_home;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel panel7;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Panel panel_settings;
        private System.Windows.Forms.Panel toolbox_panel;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button Node_add_Button;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel resize_panel;
    }
}

