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
                    txtLinks.Text += element.Attributes["href"].Value + "\n";
                }
                
                
            }
        }
    }
}