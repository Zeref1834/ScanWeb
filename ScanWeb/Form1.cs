using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using HtmlAgilityPack;

namespace ScanWeb
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        

        private void button2_Click(object sender, EventArgs e)
        {
            //using(OpenFileDialog dialog = new OpenFileDialog())
            //{
            //    dialog.Multiselect = false;
            //    dialog.InitialDirectory = @"E:\";
            //    dialog.Filter = "Text files(*.txt)|*.txt";
            //    if(dialog.ShowDialog() == DialogResult.OK)
            //    {
            //        textBox1.Text = File.ReadAllText(dialog.FileName).ToString();
            //    }    
            //}


            //if (UrlIsValid(textBox1.Text) == true)
            //    MessageBox.Show("The URL is Valid");
            //else
            //    MessageBox.Show("The URL is not Valid");
            listBox2.Items.Clear();
            GetURL(textBox1.Text);
        }
        //public bool UrlIsValid(string url)
        //{
        //    try
        //    {
        //        HttpWebRequest request = HttpWebRequest.Create(url) as HttpWebRequest;
        //        request.Timeout = 5000;
        //        request.Method = "HEAD";

        //        using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
        //        {
        //            int statusCode = (int)response.StatusCode;
        //            if (statusCode >= 100 && statusCode < 400)
        //            {
        //                return true;
        //            }
        //            else if (statusCode >= 500 && statusCode <= 510)
        //            {
        //                return false;
        //            }
        //        }
        //    }
        //    catch (WebException ex)
        //    {
        //        if (ex.Status == WebExceptionStatus.ProtocolError) //400 errors
        //        {
        //            return false;
        //        }

        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //    return false;
        //}

        public void ScanXss(string parm, string _url)
        {
            string _xssUrl = _url.Replace(parm, parm + "ra<xss>it");
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_xssUrl);
            request.Method = "GET";
            string _xss = string.Empty;
            using (StreamReader lectura = new StreamReader(request.GetResponse().GetResponseStream())) _xss = lectura.ReadToEnd();
            if (_xss.Contains("<xss>"))
            {
                listBox2.Items.Add(parm + "\t" + "Have a xss Vulnerabilities");
            }
            else
            {
                listBox2.Items.Add(parm + "\t" + "Don't have a xss Vulnerabilities");
            }
        }

        public void ScanSql(string parm, string _url)
        {
            string _sqlUrl = _url.Replace(parm, parm + "ra'it");
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_sqlUrl);
            request.Method = "GET";
            string _sql = string.Empty;
            using (StreamReader lectura = new StreamReader(request.GetResponse().GetResponseStream())) _sql = lectura.ReadToEnd();
            if (_sql.Contains("error in your SQL syntax"))
            {
                listBox2.Items.Add(parm + "\t" + "Have a Sql Injection Vulnerability");
            }
            else
            {
                listBox2.Items.Add(parm + "\t" + "Don't have a Sql Injection Vulnerability");
            }
        }

        public void GetURL(string txt)
        {
            List<string> _listUrl = new List<string>();
            HtmlWeb hw = new HtmlWeb();
            HtmlAgilityPack.HtmlDocument doc = hw.Load(txt);
            foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//a[@href]"))
            {
                HtmlAttribute att = link.Attributes["href"];
                _listUrl.Add(att.Value);
            }
            foreach (var item in _listUrl)
            {
                string _url = item.ToString();
                int index = _url.IndexOf('?');
                string[] parms = _url.Remove(0, index + 1).Split('&');

                foreach (string parm in parms)
                {
                    try 
                    {
                        if (XssCheckBox.Checked == true && SqlCheckBox.Checked == false)
                        {
                            ScanXss(parm, _url);
                        }else if(SqlCheckBox.Checked == true && XssCheckBox.Checked == false)
                        {
                            ScanSql(parm, _url);
                        }else if(XssCheckBox.Checked == true && SqlCheckBox.Checked == true)
                        {
                            ScanSql(parm, _url);
                            ScanXss(parm, _url);
                        }    
                    } 
                    catch (Exception ex) { }
                    
                }

            }
        }

        private void optionToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Close_Click(object sender, EventArgs e)
        {
            if(SqlCheckBox.Checked == false && XssCheckBox.Checked == false)
            {
                button2.Enabled = false;
                textBox1.Enabled = false;
            }
            else
            {
                button2.Enabled = true;
                textBox1.Enabled = true;
            }
            panel1.Visible = false;
            Close.Visible = false;
        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            panel1.Visible = true;
            Close.Visible = true;
        }

        private void menuToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}
