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
using System.Reflection.Metadata;
using System.IO.Compression;
using System.Net.Http;
using System.DirectoryServices.ActiveDirectory;
using System.Linq.Expressions;

namespace WebSiteScrapper
{
    public class Scraper
    {
        public string? protocol;
        public string? baseurl;
        public string? refererUrl;
        public Urls Urls;
        public HtmlDocument? HtmlPage;
        public string? hashedPage;
        public Form1 form;
        public int totalLinks = 0;
        public int linksVisited = 0;
        public int linksYetToVisit = 0;
        public int maxYetToVisit = 0;
        public bool hasError = false;
        public WebSiteScrapperContext Context;
        public const bool _VISITED = true;
        public bool _NOTVISITED = false;
        public bool paused = false;


        public Scraper(string url, Form1 _form, WebSiteScrapperContext _Context)
        {
            this.Context = _Context;
            this.form = _form;

            this.Urls = new Urls();
            this.Urls.Url = GetAbsoluteUrlString(url, "");
            this.baseurl = this.Urls.Url;
            this.Urls.Baseurl = this.Urls.Url;
            this.refererUrl = this.Urls.Url;
            this.HtmlPage = this.GetPage();
            this.GetTitle();            
            this.GetLinks();
            this.GetUrlProtocol(url);

           
        }
        public async static Task<string> GetPage_V2(string uri)
        {
            string content = null;

            var client = new HttpClient();
            var response = await client.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                content = await response.Content.ReadAsStringAsync();
            }            

            return content;
        }


        public HtmlDocument GetPage(string url="")
        {
            var htmlDoc = new HtmlDocument();
            using (var client = new WebClient())
            {
                try
                {
                    if (url == "")
                        htmlDoc.LoadHtml(client.DownloadString(this.Urls.Url).Trim());
                    else
                        htmlDoc.LoadHtml(client.DownloadString(url).Trim());

                    //this.Urls.Page = htmlDoc.Text;
                    this.Urls.Page = htmlDoc.ParsedText;
                }
                catch (Exception ex)
                {
                    this.Urls.Title = ex.Message + this.Urls.Url;
                    htmlDoc.LoadHtml("");
                    this.Urls.Page = "";
                    hasError = true;
                }

            }



            this.HtmlPage = htmlDoc;
            return htmlDoc;
        }                 
        public string? GetTitle()
        {            
            if (this.HtmlPage != null && this.HtmlPage.DocumentNode.SelectSingleNode("//title") != null)
            {
                this.Urls.Title = HtmlPage.DocumentNode.SelectSingleNode("//title").InnerText.ToString();
            }

            return this.Urls.Title;
        }
        public List<HtmlNode>? GetLinks()
        {
            if (this.HtmlPage != null && this.HtmlPage.DocumentNode.SelectNodes("//a") == null)
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
            this.CleanTable("urls");

            //Step 1 Add the base url to the database and mark it as visited                                    
            AddUrlToDb(this.Urls.Url, _VISITED);

            //Step 2 Get all the urls from the BasePage            
            //Step 3 Add the urls in the database
            AddUrlsToDb(GetLinks());

            UpdateControls();            

            //Step 4 Select the next url that has not been visited 
            this.Urls = GetNextUrlFromDb();


            //Step 5 While there is an unvisited url
            while (this.Urls != null)
            {
                if(this.paused == false)
                {
                    //Step 5.1 Initialize the new url
                    //this.Urls.Url = FixUrl(this.Urls.Url);
                    this.Urls.Url = GetAbsoluteUrlString(this.refererUrl, this.Urls.Url);
                    

                    if (!this.hasError)
                    {
                        //Step 5.2 Get the page
                        GetPage();
                        //Step 5.3 Get the title
                        GetTitle();
                    }

                    this.hasError = false;
                    //Step 5.4 Mark the visited url as visited
                    UpdateUrlAsVisited(this.Urls.Id);
                    //Step 5.5 Get all the unvisited link urls from the page and add them to the database
                    this.refererUrl = this.Urls.Url;
                    AddUrlsToDb(GetLinks());

                    //Step 5.6 Get the next url from the database
                    this.Urls = GetNextUrlFromDb();

                    //Step 5.7 Update the counters
                    UpdateControls();
                    //this.form.SetlabelValue("Scraping on " + this.Urls.Url, calculateTotal().ToString(), calculateYetToVisit().ToString(), calculateVisited().ToString(), this.maxYetToVisit.ToString());

                    //Step 5.8 go to step 5
                }
            }


            return this.GetAllDbUrls();
        }        
        public void UpdateControls()
        {
            string message = "";
            if (this.Urls != null)
                message = "Scraping on " + this.Urls.Url;
            else
                message = "finished scraping all urls";

            this.form.SetlabelValue(message, CalculateTotal().ToString(), CalculateYetToVisit().ToString(), CalculateVisited().ToString(), this.maxYetToVisit.ToString());

        }
        public Urls GetNextUrlFromDb()
        {
            //WebSiteScrapperContext context = new WebSiteScrapperContext();
            var url = this.Context.Urls.Where(s => s.Visited == false)                                          
                        .FirstOrDefault();            

            return url;
        }
        public int CalculateVisited()
        {
            //WebSiteScrapperContext context = new WebSiteScrapperContext();
            var url = this.Context.Urls.Where(s => s.Visited == true).ToList();

            return url.Count();
        }
        public int CalculateYetToVisit()
        {
            //WebSiteScrapperContext context = new WebSiteScrapperContext();
            var url = this.Context.Urls.Where(s => s.Visited == false).ToList();

            if (maxYetToVisit < url.Count())
                maxYetToVisit = url.Count();

            return url.Count();
        }
        public int CalculateTotal()
        {
            //WebSiteScrapperContext context = new WebSiteScrapperContext();
            var url = this.Context.Urls.ToList();

            return url.Count();
        }
        public void CleanTable(string tableName)
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
        public void AddUrlsToDb(List<HtmlNode>? links)
        {
            if(links != null)
            {
                foreach (var link in links)
                {
                    //string _url = FixUrl(link.Attributes["href"].Value);
                    string _url = GetAbsoluteUrlString(this.refererUrl, link.Attributes["href"].Value);
                    if (!IsInDb(_url) && IsUrlInternal(_url))
                    {
                        this.AddUrlToDb(_url, _NOTVISITED);
                    }
                }
            }            
        }

        public Boolean IsUrlInternal_V2(string url)
        {
            bool match = false;
            try
            {
                var uri = new Uri(url);
                var uri2 = new Uri(this.baseurl);
                match = uri.Host.Equals(uri2.Host, StringComparison.InvariantCultureIgnoreCase);
            }
            catch(Exception ex)
            {

            }
            


            return match;
        }
        public Boolean IsUrlInternal(string url)
        {
            if((this.IsUrlInternal_V2(url)) || !(url.ToLower().StartsWith("http://") || url.ToLower().StartsWith("https://")))
            {
                return true;
            }

            return false;
        }
        public Boolean IsInDb(string url)
        {            
            Urls lurl = this.Context.Urls.Where(s => s.Url == url).FirstOrDefault();
            return lurl != null ? true : false;            
        }
        public void AddUrlToDb(string tmpUrl, Boolean visited)
        {        
            this.Context.ChangeTracker.Clear();
            this.Urls.Id = 0;            
            this.Urls.Baseurl = this.baseurl;

            this.Urls.Url = tmpUrl;
            this.Urls.Date = DateTime.Now;
            this.Urls.Hash = "";
            this.Urls.Visited = visited;
            this.Urls.RefererUrl = this.refererUrl;

            if (this.Urls != null)
            {
                
                this.Context.Add(this.Urls);
                this.Context.SaveChanges();
            }
            
        }
        public void UpdateUrlAsVisited(long id)
        {
            this.Context.ChangeTracker.Clear();
            //this.Urls.Baseurl = this.Urls.Url;
            this.Urls.Date = DateTime.Now;
            this.Urls.Hash = "";
            this.Urls.Visited = true;

            if (this.Urls != null)
            {
                this.Context.Update(this.Urls);
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
        public void GetUrlProtocol(string url)
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
        public string FixUrl(string url)
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

        public string GetAbsoluteUrlString(string baseUrl, string url)
        {
            var uri = new Uri(url, UriKind.RelativeOrAbsolute);
            if (!uri.IsAbsoluteUri)
                uri = new Uri(new Uri(baseUrl), uri);
            return uri.ToString();
        }
        public List<Urls> GetAllDbUrls()
        {
            //WebSiteScrapperContext context = new WebSiteScrapperContext();
            var url = this.Context.Urls.ToList();

            return url;
        }
        public void Pause()
        {
            this.paused = true;
        }

        public void Start()
        {
            this.paused = false;
        }

    }
}
