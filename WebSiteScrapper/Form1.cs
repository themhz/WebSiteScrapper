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
            var htmlDoc = scraper.Scrape();

            // Extract specific data from the website using HtmlAgilityPack methods
            var title = htmlDoc.DocumentNode.SelectSingleNode("//head/title").InnerText;
            MessageBox.Show(title);
            
            foreach(var element in htmlDoc.DocumentNode.SelectNodes("//a"))
            {
                if(element.Attributes["href"]!= null)
                {
                    txtLinks.Text += element.Attributes["href"].Value + " - " + HashString(element.Attributes["href"].Value) + "\n";
                }
                
                
            }
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