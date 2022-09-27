using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ghost
{
    public partial class SignedMessageShow : Form
    {
        string message;
        string open_key;
        string date;
        string sign;

        public SignedMessageShow(string open_key, string username, string sign, string message, string date)
        {
            InitializeComponent();

            this.message = message;
            this.open_key = open_key;
            this.date = date;
            this.Text = username;
            this.sign = sign;
        }

        private void SignedMessageShow_Load(object sender, EventArgs e)
        {
            BackColor = Main.FormBackColor;
            MinimizeBox = false;
            MaximizeBox = false;
            ShowIcon = false;
            tableLayoutPanel1.BackColor = Main.FormBackColor;

            richTextBox1.BackColor = Main.CustomBackColor;
            richTextBox2.BackColor = Main.CustomBackColor;
            richTextBox3.BackColor = Main.CustomBackColor;

            richTextBox1.BorderStyle = BorderStyle.None;
            richTextBox2.BorderStyle = BorderStyle.None;
            richTextBox3.BorderStyle = BorderStyle.None;

            richTextBox1.Font = new Font("Aire Exterior", 12);
            richTextBox1.ForeColor = Main.CustomForeColor;
            richTextBox2.Font = new Font("Aire Exterior", 12);
            richTextBox2.ForeColor = Main.CustomForeColor;
            richTextBox3.Font = new Font("Aire Exterior", 12);
            richTextBox3.ForeColor = Main.CustomForeColor;

            label1.Font = new Font("Aire Exterior", 12);
            label1.ForeColor = Main.CustomForeColor;
            label2.Font = new Font("Aire Exterior", 12);
            label2.ForeColor = Main.CustomForeColor;
            label3.Font = new Font("Aire Exterior", 12);
            label3.ForeColor = Main.CustomForeColor;

            label1.Text = "Публичный ключ";
            label2.Text = "Подпись";
            label3.Text = "Сообщение " + date;

            richTextBox1.Text = open_key;
            richTextBox2.Text = sign;
            richTextBox3.Text = message;

            richTextBox1.ReadOnly = true;
            richTextBox2.ReadOnly = true;
            richTextBox3.ReadOnly = true;

            if (richTextBox2.Text != "")
                check_sign();
        }

        private void check_sign()
        {
            UnicodeEncoding converter = new UnicodeEncoding();
            byte[] plainText = converter.GetBytes(message);

            
            // Generate the public key/these can be sent to the user. 
            var publicParams = Convert.FromBase64String(open_key);
            
            byte[] signature = Convert.FromBase64String(sign);

            // Verify from the user's side. Note that only the public parameters 
            // are needed. 
            var rsaRead = new RSACryptoServiceProvider();

            rsaRead.ImportCspBlob(publicParams);

            if (rsaRead.VerifyData(plainText,
                     new SHA256CryptoServiceProvider(),
                     signature))
            {
                label2.ForeColor = Color.Green;
                label2.Text += " (Подтвержденна)";
            }
            else
            {
                label2.ForeColor = Color.Red;
                label2.Text += " (Несоответствие)";
            }
        }
    }
}
