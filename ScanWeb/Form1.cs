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
using ScanWeb.Model;

namespace ScanWeb
{
    public partial class Form1 : Form
    {
        private List<string> _listRequest = new List<string>();
        private int numberOfXSS = 0;
        private int numberOfSQL = 0;
        List<UrlDetailModel> _listUrlDetail = new List<UrlDetailModel>();
        public Form1()
        {
            InitializeComponent();

        }

        

        private void button2_Click(object sender, EventArgs e)
        {
            
            listBox2.Items.Clear();
            GetURL(textBox1.Text);
            AddTreeView();
        }

        public void ScanXss(string parm, string _url)
        {
            string _xssUrl = _url.Replace(parm, parm + "ra<xss>it");
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_xssUrl);
            request.Method = "GET";
            string _xss = string.Empty;
            using (StreamReader lectura = new StreamReader(request.GetResponse().GetResponseStream())) _xss = lectura.ReadToEnd();
            if (_xss.Contains("<xss>"))
            {
                numberOfXSS ++;
                UrlDetailModel item = new UrlDetailModel(_url, _xss);
                _listUrlDetail.Add(item);
            }
        }

        public void ScanSql(string parm, string _url)
        {
            //string _sqlUrl = _url.Replace(parm, parm + "ra'it");
            //HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_sqlUrl);
            //request.Method = "GET";
            //string _sql = string.Empty;
            //using (StreamReader lectura = new StreamReader(request.GetResponse().GetResponseStream())) _sql = lectura.ReadToEnd();
            //if (_sql.Contains("error in your SQL syntax"))
            //{
            //    numberOfSQL ++;
            //    _listSQL.Add(parm);
            //}
        }

        public async void GetURL(string txt)
        {
            List<string> _listUrl = new List<string>();
            HtmlWeb hw = new HtmlWeb();
            string[] parms;
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
                parms = _url.Remove(0, index + 1).Split('&');
                listBox2.Items.Add(_url);
                foreach (string parm in parms)
                {
                    try
                    {
                        if (XssCheckBox.Checked == true && SqlCheckBox.Checked == false)
                        {
                            ScanXss(parm, _url);
                        }
                        else if (SqlCheckBox.Checked == true && XssCheckBox.Checked == false)
                        {
                            ScanSql(parm, _url);
                        }
                        else if (XssCheckBox.Checked == true && SqlCheckBox.Checked == true)
                        {
                            ScanSql(parm, _url);
                            ScanXss(parm, _url);
                        }
                    }
                    catch (Exception ex) { }

                }
            }
        }

        private void AddTreeView()
        {
            treeView1.Nodes.Add("XSS Injections: " + numberOfXSS);
            treeView1.Nodes.Add("SQL Injections: " + numberOfSQL);
            if (numberOfXSS != 0 )
            {
                foreach(var item in _listUrlDetail)
                {
                    treeView1.Nodes[0].Nodes.Add(item.Url);
                }
            }
            if(numberOfSQL != 0)
            {
                foreach (var item in _listUrlDetail)
                {
                    treeView1.Nodes[1].Nodes.Add(item.Url);
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

        private void ShowResponse(object sender, EventArgs e)
        {
            
        }
    }
}
