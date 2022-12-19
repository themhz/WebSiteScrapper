using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using HtmlAgilityPack;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;

namespace WebSiteScrapper
{
    public class Scraper
    {
        private readonly string url;

        public Scraper(string url)
        {
            this.url = url;
        }

        public HtmlDocument Scrape()
        {
            var htmlDoc = new HtmlDocument();
            using (var client = new WebClient())
            {
                var html = client.DownloadString(url);
                htmlDoc.LoadHtml(html);
            }
            return htmlDoc;
        }
    }
}
