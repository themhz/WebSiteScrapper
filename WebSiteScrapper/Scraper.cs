using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using HtmlAgilityPack;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;
using System.Diagnostics;

namespace WebSiteScrapper
{
    public class Scraper
    {
        private readonly string baseurl;
        private String url;
        private List<string> urls;

        public Scraper(string url)
        {
            //Initialize the base url
            this.baseurl = url;
            this.url = url;
            this.urls = new List<string>();
            this.urls.Add(url);
            
        }
        public HtmlDocument Scrape()
        {
            var htmlDoc = new HtmlDocument();
            using (var client = new WebClient())
            {
                try
                {
                    var html = client.DownloadString(url);
                    htmlDoc.LoadHtml(html);
                }
                catch (Exception ex)
                {
                    this.urls.Add("url not found : " + url);
                }
                
            }
            return htmlDoc;
        }        
        public List<HtmlNode> GetUrls(HtmlDocument htmlDoc)
        {            
            List<HtmlNode> urls = new List<HtmlNode>();
            foreach (var element in htmlDoc.DocumentNode.SelectNodes("//a"))
            {                
                urls.Add(element);             
            }            
            return urls;
        }
        public List<string> GetAllUrlsFromSite()
        {
            for (int i = 0; i < urls.Count; i++)
            {
                this.url = urls[i];
                HtmlDocument doc = this.Scrape();

                if (doc.DocumentNode.SelectNodes("//a") != null)
                {
                    foreach (var element in doc.DocumentNode.SelectNodes("//a"))
                    {

                        //if they are not in the list add them
                        if (element.Attributes.Contains("href") && !this.urls.Contains(element.Attributes["href"].Value))
                        {
                            string tmpUrl = element.Attributes["href"].Value;

                            if (!(tmpUrl.Contains("http://") || tmpUrl.Contains("https://")) && (tmpUrl.StartsWith("/") || tmpUrl.StartsWith("\"")))
                            {
                                tmpUrl = this.baseurl + tmpUrl;
                                if (!this.urls.Contains(tmpUrl)){
                                    this.urls.Add(tmpUrl);
                                }
                                
                            }
                            else
                            {
                                if (tmpUrl.StartsWith(baseurl) || tmpUrl.StartsWith(baseurl.Replace("http", "https")))
                                {
                                    if (!this.urls.Contains(tmpUrl)){
                                        this.urls.Add(tmpUrl);
                                    }
                                }
                            }


                        }

                    }
                }
                
            }
                
            
            

            return this.urls;
        }
    }
}
