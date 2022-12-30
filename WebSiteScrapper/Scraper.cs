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
        private Urls Urls;        
        private HtmlDocument? HtmlPage;
        private string? hashedPage;        
        private Form1 form;
        private int totalLinks = 0;
        private int linksVisited = 0;
        private int linksYetToVisit = 0;
        private int maxYetToVisit = 0;
        private bool hasError = false;
        public WebSiteScrapperContext Context;



        public Scraper(string url, Form1 _form, WebSiteScrapperContext _Context)
        {
            this.Urls = new Urls();            
            this.Urls.Url = url;
            this.baseurl = url;            
            this.form = _form;
            this.initialiseScraper(url);
            this.getUrlProtocol(url);
            this.Context = _Context;
        }
        //Initialize the base url
        public void initialiseScraper(string url)
        {
            this.Urls.Url = url;            
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
                    var html = client.DownloadString(this.Urls.Url);                    
                    this.hashedPage = this.HashString(html);
                    htmlDoc.LoadHtml(html);
                    HtmlPage = htmlDoc;
                }
                catch (Exception ex)
                {                    
                    this.Urls.Title = "URL NOT FOUND : " + this.Urls.Url;
                    this.hashedPage = "";
                    hasError = true;

                }
                
            }
            return htmlDoc;
        }                 
        public string? GetTitle()
        {            
            if (HtmlPage.DocumentNode.SelectSingleNode("//title") != null)
            {
                this.Urls.Title = HtmlPage.DocumentNode.SelectSingleNode("//title").InnerText.ToString();
            }

            return this.Urls.Title;
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
            
        public List<Urls> GetAllUrlsFromSite_v2()
        {
            //Helper just cleans the urls
            this.cleanTable("urls");

            //Step 1 Add the base url to the database and mark it as visited                        
            GetTitle();
            addUrlToDb(this.Urls.Url, true);

            //Step 2 Get all the urls from the BasePage
            List<HtmlNode>? links = GetLinks();

            //Step 3 Add the urls in the database
            addUrlsToDb(links);
            this.form.SetlabelValue("Initializing " + this.Urls.Url, calculateTotal().ToString(), calculateYetToVisit().ToString(), calculateVisited().ToString(), this.maxYetToVisit.ToString());

            //Step 4 Select the next url that has not been visited 
            this.Urls = getNextUrlFromDb();


            //Step 5 While there is an unvisited url
            while (this.Urls != null)
            {
                //Step 5.1 Initialize the new url
                this.Urls.Url = fixUrl(this.Urls.Url);
                //Step 5.2 Get the page
                GetHtmlPage();

                if (!this.hasError)
                {
                    //Step 5.3 Get the title
                    GetTitle();
                }

                this.hasError = false;
                //Step 5.4 Mark the visited url as visited
                updateUrlAsVisited(this.Urls.Id);
                //Step 5.5 Get all the unvisited link urls from the page and add them to the database
                addUrlsToDb(GetLinks());

                //Step 5.6 Get the next url from the database
                this.Urls = getNextUrlFromDb();

                //Step 5.7 Update the counters
                updateControls();
                //this.form.SetlabelValue("Scraping on " + this.Urls.Url, calculateTotal().ToString(), calculateYetToVisit().ToString(), calculateVisited().ToString(), this.maxYetToVisit.ToString());

                //Step 5.8 go to step 5

            }


            return this.GetAllDbUrls();
        }        
        private void updateControls()
        {
            string message = "";
            if (this.Urls != null)
                message = "Scraping on " + this.Urls.Url;
            else
                message = "finished scraping all urls";

            this.form.SetlabelValue(message, calculateTotal().ToString(), calculateYetToVisit().ToString(), calculateVisited().ToString(), this.maxYetToVisit.ToString());

        }
        private void updateUrlAsVisited(long id)
        {                       
            this.Context.ChangeTracker.Clear();
            this.Urls.Baseurl = this.Urls.Url;
            this.Urls.Date = DateTime.Now;
            this.Urls.Hash = this.hashedPage;
            this.Urls.Visited = true;

            if (this.Urls != null)
            {
                this.Context.Update(this.Urls);
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
                    string _url = fixUrl(link.Attributes["href"].Value);
                    if (!isInDb(_url) && isUrlInternal(_url))
                    {
                        this.addUrlToDb(_url, false);
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
            this.Context.ChangeTracker.Clear();
            this.Urls.Id = 0;            
            this.Urls.Baseurl = this.Urls.Url;
            this.Urls.Url = tmpUrl;
            this.Urls.Date = DateTime.Now;
            this.Urls.Hash = this.hashedPage;
            this.Urls.Visited = visited;

            if (this.Urls != null)
            {
                
                this.Context.Add(this.Urls);
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
        public string fixUrl(string url)
        {
            string _url = url;
            if (url.ToLower().StartsWith("http://") || url.ToLower().StartsWith("https://"))
            {
                _url = url;
            }
            else if (!(url.ToLower().StartsWith("http://") || url.ToLower().StartsWith("https://")))
            {
                if (baseurl.ToLower().EndsWith("/") && url.ToLower().StartsWith("/"))
                {
                    _url = baseurl.Substring(0, baseurl.Length - 1) + url;
                }
                else if (baseurl.ToLower().EndsWith("/") && url.ToLower().StartsWith("//"))
                {
                    _url = baseurl.Substring(0, baseurl.Length - 1) + url.Substring(1, baseurl.Length);
                }
                else if (baseurl.ToLower().EndsWith("/") && url.ToLower().StartsWith("\""))
                {
                    _url = baseurl.Substring(0, baseurl.Length) + url.Substring(1, baseurl.Length);
                }
                else if (baseurl.ToLower().EndsWith("/") && !url.ToLower().StartsWith("/"))
                {
                    _url = baseurl + url;
                }
                else if (!baseurl.ToLower().EndsWith("/") && url.ToLower().StartsWith("/"))
                {
                    _url = baseurl + url;
                }
                else if (!baseurl.ToLower().EndsWith("/") && !url.ToLower().StartsWith("/"))
                {
                    _url = baseurl + "/" + url;
                }

            }
            _url = _url.ToLower().Trim();

            if (_url.Trim().EndsWith("/"))
            {
                _url = _url.ToLower().Trim().Substring(0, _url.Length - 1);
            }

            return _url;                                                                   
        }
        public List<Urls> GetAllDbUrls()
        {
            //WebSiteScrapperContext context = new WebSiteScrapperContext();
            var url = this.Context.Urls.ToList();

            return url;
        }

        
    }
}
