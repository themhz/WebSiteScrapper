using AngleSharp.Html;
using AngleSharp.Html.Parser;
using HtmlAgilityPack;
using Microsoft.Web.WebView2.Core;
using ServiceStack.Html;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using WebSiteScrapper.Classes;
using WebSiteScrapper.Data;
using WebSiteScrapper.Models;
using Webview;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;

namespace WebSiteScrapper
{
    public partial class Browser : Form
    {
        public Browser()
        {            
            InitializeComponent();
            listBox1.KeyDown += new KeyEventHandler(listBox1_KeyDown);
        }

        private void listBox1_KeyDown(object sender, KeyEventArgs e)
        {
            // Check if the user pressed the "Ctrl+C" keys
            if (e.Control && e.KeyCode == Keys.C)
            {
                // Copy the selected items to the clipboard
                string items = "";
                foreach (var item in listBox1.SelectedItems)
                {
                    items += item.ToString() + Environment.NewLine;
                }
                Clipboard.SetText(items);
            }
        }
        private async void button1_Click(object sender, EventArgs e)
        {

            //firstTask();
            //Task.Run(async () => SecondTask());
            //Task.Run(async () => ThirdTask());
            SecondTask();
            //ThirdTask();
        }

        public async Task SecondTask()
        {
            Spider s = new Spider();
            string url = "https://services.tee.gr/auth/faces/appMain";
           string page = s.GetPageAsString(url);
           richTextBox1.Text = page;

            //await webView21.EnsureCoreWebView2Async();
            //webView21.NavigateToString(page);
            //webView21.NavigateToString("https://services.tee.gr/auth/faces/appMain");


            webView21.Source = new Uri("https://services.tee.gr/auth/faces/appMain", UriKind.Absolute);
            
            webView21.NavigationCompleted += Web_NavigationCompleted;
            
        }
        private async void Web_NavigationCompleted(object? sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            Thread.Sleep(1000);
            richTextBox1.Text = await webView21.ExecuteScriptAsync("document.documentElement.outerHTML;");
            //Go to page and Login
            var txtusername = "200007";
            var txtpassword = "cc200007";
            var username = await webView21.ExecuteScriptAsync($"document.getElementById('username').value = '{txtusername}';");
            var password = await webView21.ExecuteScriptAsync($"document.getElementById('password').value = '{txtpassword}';");
            


            Thread.Sleep(1000);
            await webView21.ExecuteScriptAsync("document.getElementsByClassName('formButton')[0].click();");
            webView21.NavigationCompleted += Web_NavigationCompleted2;
            

        }

        private async void Web_NavigationCompleted2(object? sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            //Navigate to the page
            Thread.Sleep(1000);
            richTextBox1.Text = await webView21.ExecuteScriptAsync("document.documentElement.outerHTML;");
            Thread.Sleep(1000);
            await webView21.ExecuteScriptAsync("document.getElementById('r1:0:np1:cni1').click();");
            webView21.NavigationCompleted += Web_NavigationCompleted3;
            
        }

        private async void Web_NavigationCompleted3(object? sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            //Navigate to the page
            Thread.Sleep(1000);
            richTextBox1.Text = await webView21.ExecuteScriptAsync("document.documentElement.outerHTML;");
            Thread.Sleep(1000);
            await webView21.ExecuteScriptAsync("document.getElementsByClassName('xfp')[0].click();");
            webView21.NavigationCompleted += Web_NavigationCompleted4;
            
        }

        private async void Web_NavigationCompleted4(object? sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            //Navigate to the page
            Thread.Sleep(1000);
            richTextBox1.Text = await webView21.ExecuteScriptAsync("document.documentElement.outerHTML;");
            Thread.Sleep(1000);
            await webView21.ExecuteScriptAsync("document.getElementById('r1:1:resId1:481:resId1c1').click();");
            webView21.NavigationCompleted += Web_NavigationCompleted5;
        }

        private async void Web_NavigationCompleted5(object? sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            Thread.Sleep(1000);
            richTextBox1.Text = await webView21.ExecuteScriptAsync("document.documentElement.outerHTML;");

        }



        public async Task firstTask()
        {
            await webView21.EnsureCoreWebView2Async();

            WebSiteScrapperContext _Context = new WebSiteScrapperContext(ConfigurationManager.ConnectionStrings["MyDbConnection"].ConnectionString);
            Hive h = new Hive(_Context, null);
            SqlHandler sh = new SqlHandler(_Context);

            //string? htmlContent = sh.GetUrlByUrlField("https://www.civiltech.gr/Blog/Default.aspx").Page.ToString();
            string? htmlContent = sh.GetUrlByUrlField("https://www.civiltech.gr/Blog/Default.aspx").Page.ToString();

            webView21.NavigateToString(htmlContent);

            string formattedOutput = "";

            var parser = new HtmlParser();

            var document = parser.ParseDocument(htmlContent);

            var sw = new StringWriter();
            document.ToHtml(sw, new PrettyMarkupFormatter());

            var HTML_prettified = sw.ToString();


            richTextBox1.Text = HTML_prettified;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            getSource();
        }

        private async void getSource()
        {
            richTextBox1.Text = await webView21.ExecuteScriptAsync("document.documentElement.outerHTML;");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            webView21.Reload();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            getControls();

        }
        private async void getControls()
        {
            var source = await webView21.ExecuteScriptAsync("document.documentElement.outerHTML;");
            source = Regex.Unescape(source);
            source = source.Remove(0, 1);
            source = source.Remove(source.Length - 1, 1);

            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(source);
            var inputElements = htmlDoc.DocumentNode.Descendants("input").ToList();            
            var changedInputs = new List<HtmlNode>();
            foreach (var input in inputElements)
            {                                             
                //changedInputs.Add(input);
                listBox1.Items.Add(input.OuterHtml);
            }
            listBox1.Items.Add("--------------------------------------------");
        }

        private void Browser_Load(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
        }
    }
}
