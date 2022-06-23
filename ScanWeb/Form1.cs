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
        private string[] xssParameters;
        private string[] sqlParameters;
        private string[] xssAttributes;
        private string[] sqlAttributes;


        public Form1()
        {
            InitializeComponent();
            GetParameter();
            GetAttributes();
        }
        public void GetParameter()
        {
            string textFileXss = @"C:\Users\PC\OneDrive\Desktop\DATN\ScanWeb\ScanWeb\String\XssPayloadList.txt";
            xssParameters = File.ReadAllLines(textFileXss);
            string textFileSql = @"C:\Users\PC\OneDrive\Desktop\DATN\ScanWeb\ScanWeb\String\SqlInjectionsPayload.txt";
            sqlParameters = File.ReadAllLines(textFileSql);
        }
        public void GetAttributes()
        {
            string textFileXss = @"C:\Users\PC\OneDrive\Desktop\DATN\ScanWeb\ScanWeb\String\Xss_attributes.txt";
            xssAttributes = File.ReadAllLines(textFileXss);
            string textFileSql = @"C:\Users\PC\OneDrive\Desktop\DATN\ScanWeb\ScanWeb\String\SqlInjection_attributes.txt";
            sqlAttributes = File.ReadAllLines(textFileSql);
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            numberOfSQL = 0;
            numberOfXSS = 0;
            treeView1.Nodes.Clear();
            dataGridView1.Rows.Clear();
            _listUrlDetail.Clear();
            richTextBox1.Text = null;
            richTextBox2.Text = null;
            await GetURL(textBox1.Text);
            AddTreeView();
        }

        public async Task<UrlDetailModel> ScanXssAsync(string _url, string parameter)
        {
              
            string _xssUrl = _url + parameter;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_xssUrl);
            request.Method = "GET";
            string _xss = string.Empty;
            var ws = await request.GetResponseAsync();
            StreamReader response = new StreamReader(ws.GetResponseStream());
            _xss = response.ReadToEnd();
            UrlDetailModel item = new UrlDetailModel(request.Method, _url, parameter, _xss);
            return item;

        }

        public async Task<UrlDetailModel> ScanSqlAsync(string _url, string parameter)
        {
            string _sqlUrl = _url + parameter;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_sqlUrl);
            request.Method = "GET";
            string _sql = string.Empty;
            var ws = await request.GetResponseAsync();
            StreamReader response = new StreamReader(ws.GetResponseStream());
            _sql = response.ReadToEnd();
            UrlDetailModel item = new UrlDetailModel(request.Method, _url, parameter, _sql);
            return item;
        }
        public void CheckXSS(UrlDetailModel xss)
        {
            foreach(var xssAttribute in xssAttributes)
            {
                if (xss.Response.Contains(xssAttribute))
                {
                    if (_listUrlDetail.Where(x => x.Url == xss.Url).Any())
                        return;
                    else
                    {
                        numberOfXSS++;
                        _listUrlDetail.Add(xss);
                        break;
                    }
                }
            }
            dataGridView1.Rows.Add(xss.Method, xss.Url, xss.Parameter);
            dataGridView1.FirstDisplayedScrollingRowIndex = dataGridView1.RowCount - 1;
        }
        public void CheckSQL(UrlDetailModel sql)
        {
            foreach(var sqlAttribute in sqlAttributes)
            {
                if (sql.Response.Contains(sqlAttribute))
                {
                    if (_listUrlDetail.Where(x => x.Url == sql.Url).Any())
                        return;
                    else
                    {
                        numberOfSQL++;
                        _listUrlDetail.Add(sql);
                        break;
                    }
                }
            }
            dataGridView1.Rows.Add(sql.Method, sql.Url, sql.Parameter);
            dataGridView1.FirstDisplayedScrollingRowIndex = dataGridView1.RowCount - 1;
        }

        public async Task GetURL(string txt)
        {
            if (txt == null)
                return;
            List<string> _listUrl = new List<string>();
            HtmlWeb hw = new HtmlWeb();
            HtmlAgilityPack.HtmlDocument doc = hw.Load(txt);
            foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//a[@href]"))
            {
                HtmlAttribute att = link.Attributes["href"];
                _listUrl.Add(att.Value);
            }
            foreach (var _url in _listUrl)
            {
                try
                {
                    if (XssCheckBox.Checked == true && SqlCheckBox.Checked == false)
                    {
                        foreach (string parameter in xssParameters)
                        {
                            UrlDetailModel xss = await ScanXssAsync(_url, parameter);
                            CheckXSS(xss);
                        }

                    }
                    else if (SqlCheckBox.Checked == true && XssCheckBox.Checked == false)
                    {
                        foreach(string parameter in sqlParameters)
                        {
                            UrlDetailModel sql = await ScanSqlAsync(_url, parameter);
                            CheckSQL(sql);
                        }
                    }
                    else if (XssCheckBox.Checked == true && SqlCheckBox.Checked == true)
                    {
                        foreach (string parameter in xssParameters)
                        {
                            UrlDetailModel xss = await ScanXssAsync(_url, parameter);
                            CheckXSS(xss);
                        }
                        foreach (string parameter in sqlParameters)
                        {
                            UrlDetailModel sql = await ScanSqlAsync(_url, parameter);
                            CheckSQL(sql);
                        }
                    }
                }
                catch (Exception ex) { }
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

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void Close_Click(object sender, EventArgs e)
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

        private void ShowResponse(object sender, TreeViewEventArgs e)
        {
            foreach (var unit in _listUrlDetail)
            {
                if (treeView1.SelectedNode != null && treeView1.SelectedNode.Text.Equals(unit.Url))
                {
                    richTextBox1.Text = unit.Response;
                }
                if(treeView1.SelectedNode.Text.Contains("SQL") && numberOfSQL != 0)
                {
                    string textFile = @"C:\Users\PC\OneDrive\Desktop\DATN\ScanWeb\ScanWeb\String\InfoSQL.txt";
                    richTextBox2.Text = File.ReadAllText(textFile);
                }
                if(treeView1.SelectedNode.Text.Contains("XSS") && numberOfXSS != 0)
                {
                    string textFile = @"C:\Users\PC\OneDrive\Desktop\DATN\ScanWeb\ScanWeb\String\InfoXSS.txt";
                    richTextBox2.Text = File.ReadAllText(textFile);
                }  
            }
        }
    }
}
