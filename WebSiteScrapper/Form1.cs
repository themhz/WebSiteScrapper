using HtmlAgilityPack;
using Microsoft.VisualBasic.ApplicationServices;
using System.Data;
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
using static System.Formats.Asn1.AsnWriter;
using System.Configuration;
using System.Data.SqlClient;
using WebSiteScrapper.Data;
using WebSiteScrapper.Models;


namespace WebSiteScrapper
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private string url;
        private void button1_Click(object sender, EventArgs e)
        {

            url = "https://www.in.gr/";
            var scraper = new Scraper(url);
            List<string> urls = scraper.GetAllUrlsFromSite();

//###############
//            string connectionString = ConfigurationManager.ConnectionStrings["MyDbConnection"].ConnectionString;

//            SqlHandler sh = new SqlHandler(connectionString);

//            DataSet dataSet = sh.Select("select * from urls");

//            dgrView.DataSource = dataSet.Tables[0];
//            dgrView.Update();
//###############

            //foreach (DataRow row in dataSet.Tables[0].Rows)
            //{
            //    foreach (DataColumn column in dataSet.Tables[0].Columns)
            //    {
            //        Debug.WriteLine(row[column]);
            //    }
            //}





            //List<HtmlNode> nodes = scraper.GetUrls(scraper.Scrape());

            //foreach (string node in urls)
            //{
            //    Debug.WriteLine(node);
            //}
            //DataTable dt = new DataTable();


            // Scrape the website
            //HtmlAgilityPack.HtmlDocument htmlDoc = scraper.Scrape();


            // Extract specific data from the website using HtmlAgilityPack methods
            //var title = htmlDoc.DocumentNode.SelectSingleNode("//head/title").InnerText;
            //lblUrl.Text = title + " - " + url;

            //dt = CreateDataTable(htmlDoc);

            //dgrView.DataSource = dt;
            //dgrView.Update();

        }

        private DataTable CreateDataTable(HtmlAgilityPack.HtmlDocument htmlDoc)
        {
            // Create a new DataTable.
            DataTable table = new DataTable();

            // Add two columns to the DataTable.
            table.Columns.Add("Index", typeof(int));
            table.Columns.Add("hash", typeof(string));
            table.Columns.Add("url", typeof(string));

            // Add some data to the DataTable.
            int i = 0;
            foreach (var element in htmlDoc.DocumentNode.SelectNodes("//a"))
            {
                
                if (element.Attributes["href"] != null)
                {
                    i++;
                    //txtLinks.Text += element.Attributes["href"].Value + " - " + HashString(element.Attributes["href"].Value) + "\n";
                    table.Rows.Add(i, HashString(element.Attributes["href"].Value), element.Attributes["href"].Value);
                }
            }

            return table;
        }
        private string HashString(string text, string salt = "")
        {
            if (String.IsNullOrEmpty(text))
            {
                return String.Empty;
            }

            // Uses SHA256 to create the hash
            using (var sha = new System.Security.Cryptography.SHA256Managed())
            {
                // Convert the string to a byte array first, to be processed
                byte[] textBytes = System.Text.Encoding.UTF8.GetBytes(text + salt);
                byte[] hashBytes = sha.ComputeHash(textBytes);

                // Convert back to a string, removing the '-' that BitConverter adds
                string hash = BitConverter
                    .ToString(hashBytes)
                    .Replace("-", String.Empty);

                return hash;
            }
        }
    }
}