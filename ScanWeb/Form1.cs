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
        private int numberOfXSS = 0;
        private int numberOfSQL = 0;
        private List<UrlDetailModel> _listUrlDetail = new List<UrlDetailModel>();
        public Form1()
        {
            InitializeComponent();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            GetURL(textBox1.Text);
            AddTreeView();
        }

        public void ScanXss(string parm, string _url)
        {
            string parameter = "ra<xss>it";
            string _xssUrl = _url.Replace(parm, parm + parameter);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_xssUrl);
            request.Method = "GET";
            string _xss = string.Empty;
            using (StreamReader lectura = new StreamReader(request.GetResponse().GetResponseStream())) _xss = lectura.ReadToEnd();
            if (_xss.Contains("<xss>"))
            {
                numberOfXSS ++;
                UrlDetailModel item = new UrlDetailModel(request.Method, _url, parameter, _xss);
                _listUrlDetail.Add(item);
            }
            dataGridView1.Rows.Add(request.Method, _url, parameter);
            
        }

        public void ScanSql(string parm, string _url)
        {
            string parameter = "ra'it";
            string _sqlUrl = _url.Replace(parm, parm + parameter);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_sqlUrl);
            request.Method = "GET";
            string _sql = string.Empty;
            using (StreamReader lectura = new StreamReader(request.GetResponse().GetResponseStream())) _sql = lectura.ReadToEnd();
            if (_sql.Contains("error in your SQL syntax"))
            {
                numberOfSQL++;
                UrlDetailModel item = new UrlDetailModel(request.Method, _url, parameter, _sql);
                _listUrlDetail.Add(item);
            }
            dataGridView1.Rows.Add(request.Method, _url, parameter);
        }

        public void GetURL(string txt)
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
            foreach(var unit in _listUrlDetail)
            {
                if (treeView1.SelectedNode != null &&treeView1.SelectedNode.Text.Equals(unit.Url))
                {
                    richTextBox1.Text = unit.Response;
                }    
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void Close_Click(object sender, MouseEventArgs e)
        {
            if (SqlCheckBox.Checked == false && XssCheckBox.Checked == false)
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
    }
}
