using System.Data;
using System.Reflection.Metadata.Ecma335;

namespace WebSiteScrapper
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var scraper = new Scraper("https://www.in.gr/");

            // Scrape the website
            HtmlAgilityPack.HtmlDocument htmlDoc = scraper.Scrape();

            // Extract specific data from the website using HtmlAgilityPack methods
            var title = htmlDoc.DocumentNode.SelectSingleNode("//head/title").InnerText;
            lblUrl.Text = title + " - " + "https://www.in.gr/";
            //MessageBox.Show(title);

            
            dgrView.DataSource = CreateDataTable(htmlDoc);
            dgrView.Update();

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