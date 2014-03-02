using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
//own using
using System.Web;
using System.IO;
using System.Net;
using Cotr.Service.IO;

namespace SMS_sender
{
    public partial class Form1 : Form
    {
        //personal preferences ini file loc
        string iniloc = System.AppDomain.CurrentDomain.BaseDirectory + "pre.ini";
        IniFiles inir;

        public Form1()
        {
            InitializeComponent();
        }

        private string PostWebRequest(string postUrl, string paramData, Encoding dataEncode)
        {
            string ret = string.Empty;
            try
            {
                byte[] byteArray = dataEncode.GetBytes(paramData); //转化
                HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(new Uri(postUrl));
                webReq.Method = "POST";
                webReq.ContentType = "application/x-www-form-urlencoded";

                webReq.ContentLength = byteArray.Length;
                Stream newStream = webReq.GetRequestStream();
                newStream.Write(byteArray, 0, byteArray.Length);//写入参数
                newStream.Close();
                HttpWebResponse response = (HttpWebResponse)webReq.GetResponse();
                StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.Default);
                ret = sr.ReadToEnd();
                sr.Close();
                response.Close();
                newStream.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return ret;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            inir = new IniFiles(iniloc);
            textBox1.Text = inir.ReadString("SMS_Sender", "uname", "");
            textBox2.Text = inir.ReadString("SMS_Sender", "upass", "");
            textBox3.Text = inir.ReadString("SMS_Sender", "uto", "");
            textBox4.Text = inir.ReadString("SMS_Sender", "smstemplate", "");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            richTextBox1.Text = PostWebRequest("http://quanapi.sinaapp.com/fetion.php",
                "u="+textBox1.Text+"&p="+textBox2.Text+"&to="+textBox3.Text+"&m="+textBox4.Text, Encoding.UTF8);
            if (richTextBox1.Text.IndexOf("\"result\":0") == 1)
            {
                if (checkBox1.Checked)
                {
                    inir = new IniFiles(iniloc);
                    inir.WriteString("SMS_Sender", "uname", textBox1.Text);
                    inir.WriteString("SMS_Sender", "upass", textBox2.Text);
                    inir.WriteString("SMS_Sender", "uto", textBox3.Text);
                    inir.WriteString("SMS_Sender", "smstemplate", textBox4.Text);
                }
                else
                {
                    textBox4.Text = "";
                }
                MessageBox.Show("Send OK");
            }
            else
            {
                MessageBox.Show("Send Error");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Readme r = new Readme();
            r.ShowDialog();
        }
    }
}
