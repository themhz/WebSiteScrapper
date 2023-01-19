using AngleSharp.Html;
using AngleSharp.Html.Parser;
using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WebSiteScrapper.Classes;
using WebSiteScrapper.Data;
using WebSiteScrapper.Models;
using Webview;

namespace WebSiteScrapper
{
    public partial class Browser : Form
    {
        public Browser()
        {            
            InitializeComponent();                      
        }
       

        private async void button1_Click(object sender, EventArgs e)
        {            

            await webView21.EnsureCoreWebView2Async();

            WebSiteScrapperContext _Context = new WebSiteScrapperContext(ConfigurationManager.ConnectionStrings["MyDbConnection"].ConnectionString);
            Hive h = new Hive(_Context, null);
            SqlHandler sh = new SqlHandler(_Context);

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

       
    }
}
