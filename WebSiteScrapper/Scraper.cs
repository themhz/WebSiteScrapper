using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using HtmlAgilityPack;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;
using System.Diagnostics;
using WebSiteScrapper.Data;
using WebSiteScrapper.Models;
using System.Security.Policy;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.Metrics;
using Microsoft.Identity.Client;
using System.ComponentModel;

namespace WebSiteScrapper
{
    public class Scraper
    {
        public string? protocol;
        private string? baseurl;        
        private string? url { set; get; } = "";
        private string? title;
        private HtmlDocument? HtmlPage;
        private string? hashedPage;
        private List<string> urls;
        private List<string> images;
        private Form1 form;
        private int totalLinks = 0;
        private int linksVisited = 0;
        private int linksYetToVisit = 0;
        private int maxYetToVisit = 0;
        public WebSiteScrapperContext Context;



        public Scraper(string url, Form1 _form, WebSiteScrapperContext _Context)
        {
            //Initialize the base url
            this.baseurl = url;            
            this.form = _form;
            this.initialiseScraper(url);
            this.getUrlProtocol(url);
            this.Context = _Context;
        }
        //Initialize the base url
        public void initialiseScraper(string url)
        {                        
            this.url = url;
            this.urls = new List<string>();
            this.images = new List<string>();
            GetHtmlPage();
            GetTitle();
            GetLinks();
        }
        public HtmlDocument GetHtmlPage()
        {
            var htmlDoc = new HtmlDocument();
            using (var client = new WebClient())
            {
                try
                {
                    var html = client.DownloadString(this.url);                    
                    this.hashedPage = this.HashString(html);
                    htmlDoc.LoadHtml(html);
                    HtmlPage = htmlDoc;
                }
                catch (Exception ex)
                {
                    this.urls.Add("url not found : " + url);
                }
                
            }
            return htmlDoc;
        }                 
        public string? GetTitle()
        {            
            if (HtmlPage.DocumentNode.SelectSingleNode("//title") != null)
            {
                this.title = HtmlPage.DocumentNode.SelectSingleNode("//title").InnerText.ToString();
            }

            return this.title;
        }
        public List<HtmlNode>? GetLinks()
        {
            if (HtmlPage.DocumentNode.SelectNodes("//a") == null)
            {
                return null;
            }
            else
            {
                var links = new List<HtmlNode>();
                foreach (var element in HtmlPage.DocumentNode.SelectNodes("//a"))
                {
                    if (element.Attributes.Contains("href"))
                    {
                        links.Add(element);
                    }
                }
                return links;
            }
        }

        //public List<string> GetAllUrlsFromSite()
        //{
        //    this.cleanTable("urls");
        //    int counter = 1;
        //    addUrlToDb(this.url);
            
        //    for (int i = 0; i < urls.Count; i++)
        //    {
                
        //        this.url = urls[i];                
        //        HtmlDocument doc = this.GetHtmlPage();
        //        this.title = GetTitle(doc);
        //        List <HtmlNode>? links = GetLinks(doc);

        //        this.form.SetlabelValue("Working on " + this.url);                

        //        if (links != null)
        //        {
        //            foreach (HtmlNode element in links)
        //            {
        //                //if they are not in the list add them
        //                if (!this.urls.Contains(element.Attributes["href"].Value))
        //                {
        //                    string tmpUrl = element.Attributes["href"].Value;

        //                    if(tmpUrl != null)
        //                    {
        //                        if (!(tmpUrl.StartsWith("http://") || tmpUrl.StartsWith("https://")) && (tmpUrl.StartsWith("/") || tmpUrl.StartsWith("\"")))
        //                        {
        //                            tmpUrl = this.baseurl + tmpUrl.Substring(1, tmpUrl.Length - 1);
        //                            if (!this.urls.Contains(tmpUrl))
        //                            {
        //                                this.urls.Add(tmpUrl);
        //                                counter++;
        //                                addUrlToDb(tmpUrl);
        //                            }
        //                        }
        //                        else
        //                        {
        //                            if (tmpUrl.StartsWith(baseurl) || tmpUrl.StartsWith(baseurl.Replace("http", "https")))
        //                            {
        //                                if (!this.urls.Contains(tmpUrl))
        //                                {
        //                                    this.urls.Add(tmpUrl);
        //                                    counter++;
        //                                    addUrlToDb(tmpUrl);
        //                                }
        //                            }
        //                        }
        //                    }
                            
        //                }
        //            }
        //        }                                             
        //    }
        //    this.form.SetlabelValue("Finished");
        //    return this.urls;
        //}               

        public List<String> GetAllUrlsFromSite_v2()
        {
            //Helper just cleans the urls
            this.cleanTable("urls");

            //Step 1 Add the base url to the database and mark it as visited                        
            GetTitle();
            addUrlToDb(this.url, true);

            //Step 2 Get all the urls from the BasePage
            List<HtmlNode>? links = GetLinks();

            //Step 3 Add the urls in the database
            addUrlsToDb(links);
            this.form.SetlabelValue("Initializing " + this.url, calculateTotal().ToString(), calculateYetToVisit().ToString(), calculateVisited().ToString(), this.maxYetToVisit.ToString());

            //Step 4 Select the next url that has not been visited 
            Urls urls = getNextUrlFromDb();


            //Step 5 While there is an unvisited url
            while (urls != null)
            {                
                //Step 5.1 Get all the urls from the selected url
                this.url = fixUrl(urls.Url);
                this.form.SetlabelValue("Scraping on " + this.url, calculateTotal().ToString(), calculateYetToVisit().ToString(), calculateVisited().ToString(), this.maxYetToVisit.ToString());
                initialiseScraper(this.url);
                //Step 5.2 Mark the visited url as visited
                updateUrlAsVisited(urls.Id);
                //Step 5.3 Go to Step 2                
                GetHtmlPage();
                GetTitle();                
                addUrlsToDb(GetLinks());
                urls = getNextUrlFromDb();
            }


            return this.GetAllDbUrls();
        }        
        private void updateControls()
        {
            this.form.SetlabelValue("Scraping on " + this.url, calculateTotal().ToString(), calculateYetToVisit().ToString(), calculateVisited().ToString(), this.maxYetToVisit.ToString());
        }
        private void updateUrlAsVisited(long id)
        {            
            this.Context.ChangeTracker.Clear();
            Urls url = new Urls()
            {
                Id = id,
                Title = this.title,
                Url = this.url,
                Baseurl = this.baseurl,
                Hash = this.hashedPage,
                Date = DateTime.Now,
                Visited = true
            };

            if (url != null)
            {
                this.Context.Update(url);
                this.Context.SaveChanges();
                
            }
        }
        private Urls getNextUrlFromDb()
        {
            //WebSiteScrapperContext context = new WebSiteScrapperContext();
            var url = this.Context.Urls.Where(s => s.Visited == false)                                          
                        .FirstOrDefault();

            return url;
        }
        private int calculateVisited()
        {
            //WebSiteScrapperContext context = new WebSiteScrapperContext();
            var url = this.Context.Urls.Where(s => s.Visited == true).ToList();

            return url.Count();
        }
        private int calculateYetToVisit()
        {
            //WebSiteScrapperContext context = new WebSiteScrapperContext();
            var url = this.Context.Urls.Where(s => s.Visited == false).ToList();

            if (maxYetToVisit < url.Count())
                maxYetToVisit = url.Count();

            return url.Count();
        }
        private int calculateTotal()
        {
            //WebSiteScrapperContext context = new WebSiteScrapperContext();
            var url = this.Context.Urls.ToList();

            return url.Count();
        }
        private void cleanTable(string tableName)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MyDbConnection"].ConnectionString;
            SqlConnection connection = new SqlConnection(connectionString);
            string sqlStatement = "DELETE FROM " + tableName;

            try
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand(sqlStatement, connection);
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();

            }
            finally
            {
                connection.Close();
            }
        }
        private void addUrlsToDb(List<HtmlNode> links)
        {
            if(links != null)
            {
                foreach (var link in links)
                {
                    if (!isInDb(link.Attributes["href"].Value.Trim().ToLower()) && isUrlInternal(link.Attributes["href"].Value.Trim().ToLower()))
                    {
                        this.addUrlToDb(link.Attributes["href"].Value, false);
                    }
                }
            }            
        }
        public Boolean isUrlInternal(string url)
        {
            if((url.ToLower().Replace("http://","").StartsWith(baseurl.Replace("http://", "")) 
                || url.ToLower().Replace("https://", "").StartsWith(baseurl.Replace("https://", ""))) 
                || !(url.ToLower().StartsWith("http://") || url.ToLower().StartsWith("https://"))
                )
            {
                return true;
            }

            return false;
        }
        public Boolean isInDb(string url)
        {            
            var lurl = this.Context.Urls.Where(s => s.Url == url)
                        .FirstOrDefault();

            return lurl != null ? true : false;            
        }
        private void addUrlToDb(string tmpUrl, Boolean visited)
        {
            //WebSiteScrapperContext context = new WebSiteScrapperContext();
            Urls url = new Urls()
            {            
                
                Title = this.title,
                Baseurl = this.url,
                Url = tmpUrl,
                Date = DateTime.Now,
                Hash = this.hashedPage,
                Visited = visited

            };

            if (url != null)
            {
                this.Context.Add(url);
                this.Context.SaveChanges();
            }
            
        }
        public string HashString(string text, string salt = "")
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
        public void getUrlProtocol(string url)
        {
            if (baseurl.ToLower().StartsWith("https://"))
            {
                this.protocol = "https";
            } else if (baseurl.ToLower().StartsWith("http://"))
            {
                this.protocol = "http";
            }
            else
            {
                this.protocol = "";
            }
        }
        private string fixUrl(string url)
        {
            if (url.ToLower().StartsWith("http://") || url.ToLower().StartsWith("https://"))
            {
                return url;
            }
            else if (!(url.ToLower().StartsWith("http://") || url.ToLower().StartsWith("https://")))
            {
                if (baseurl.ToLower().EndsWith("/") && url.ToLower().StartsWith("/"))
                {
                    return baseurl.Substring(0, baseurl.Length - 1) + url;
                }
                else if (baseurl.ToLower().EndsWith("/") && url.ToLower().StartsWith("//"))
                {
                    return baseurl.Substring(0, baseurl.Length - 1) + url.Substring(1, baseurl.Length);
                }
                else if (baseurl.ToLower().EndsWith("/") && url.ToLower().StartsWith("\""))
                {
                    return baseurl.Substring(0, baseurl.Length) + url.Substring(1, baseurl.Length);
                }
                else if (baseurl.ToLower().EndsWith("/") && !url.ToLower().StartsWith("/"))
                {
                    return baseurl + url;
                }
                else if (!baseurl.ToLower().EndsWith("/") && url.ToLower().StartsWith("/"))
                {
                    return baseurl + url;
                }
                else if (!baseurl.ToLower().EndsWith("/") && !url.ToLower().StartsWith("/"))
                {
                    return baseurl + "/" + url;
                }

            }            

            return url;                                                                   
        }
        public List<string> GetAllDbUrls()
        {
            //WebSiteScrapperContext context = new WebSiteScrapperContext();
            var url = this.Context.Urls.ToList();

            return null;
        }
    }
}
