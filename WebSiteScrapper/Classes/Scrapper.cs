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
using Microsoft.Extensions.DependencyInjection;

namespace WebSiteScrapper.Classes
{
    internal class Scrapper
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

        public Scrapper(WebSiteScrapperContext _Context)
        {
            Context = _Context;
        }
        public void UpdateControls()
        {
            string message = "";
            if (Urls != null)
                message = "Scraping on " + Urls.Url;
            else
                message = "finished scraping all urls";

            form.SetlabelValue(message, CalculateTotal().ToString(), CalculateYetToVisit().ToString(), CalculateVisited().ToString(), maxYetToVisit.ToString());

        }
        public Urls GetNextUrlFromDb()
        {
            //WebSiteScrapperContext context = new WebSiteScrapperContext();
            var url = Context.Urls.Where(s => s.Visited == false)
                        .FirstOrDefault();

            return url;
        }
        public int CalculateVisited()
        {
            //WebSiteScrapperContext context = new WebSiteScrapperContext();
            var url = Context.Urls.Where(s => s.Visited == true).ToList();

            return url.Count();
        }
        public int CalculateYetToVisit()
        {
            //WebSiteScrapperContext context = new WebSiteScrapperContext();
            var url = Context.Urls.Where(s => s.Visited == false).ToList();

            if (maxYetToVisit < url.Count())
                maxYetToVisit = url.Count();

            return url.Count();
        }
        public int CalculateTotal()
        {
            //WebSiteScrapperContext context = new WebSiteScrapperContext();
            var url = Context.Urls.ToList();

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
        //public void AddUrlsToDb(List<HtmlNode>? links)
        //{
        //    if (links != null)
        //    {
        //        foreach (var link in links)
        //        {
        //            //string _url = FixUrl(link.Attributes["href"].Value);
        //            string _url = GetAbsoluteUrlString(refererUrl, link.Attributes["href"].Value);
        //            if (!IsInDb(_url) && IsUrlInternal(_url))
        //            {
        //                AddUrlToDb(_url, _NOTVISITED);
        //            }
        //        }
        //    }
        //}
       
        //public bool IsUrlInternal(string url)
        //{
        //    if (IsUrlInternal_V2(url) || !(url.ToLower().StartsWith("http://") || url.ToLower().StartsWith("https://")))
        //    {
        //        return true;
        //    }

        //    return false;
        //}
        //public bool IsInDb(string url)
        //{
        //    Urls lurl = Context.Urls.Where(s => s.Url == url).FirstOrDefault();
        //    return lurl != null ? true : false;
        //}
        //public void AddUrlToDb(string tmpUrl, bool visited)
        //{
        //    Context.ChangeTracker.Clear();
        //    Urls.Id = 0;
        //    Urls.Baseurl = baseurl;

        //    Urls.Url = tmpUrl;
        //    Urls.Date = DateTime.Now;
        //    Urls.Hash = "";
        //    Urls.Visited = visited;
        //    Urls.RefererUrl = refererUrl;

        //    if (Urls != null)
        //    {

        //        Context.Add(Urls);
        //        Context.SaveChanges();
        //    }

        //}
        //public void UpdateUrlAsVisited(long id)
        //{
        //    Context.ChangeTracker.Clear();
        //    //this.Urls.Baseurl = this.Urls.Url;
        //    Urls.Date = DateTime.Now;
        //    Urls.Hash = "";
        //    Urls.Visited = true;

        //    if (Urls != null)
        //    {
        //        Context.Update(Urls);
        //        Context.SaveChanges();

        //    }

        //}
        //public string HashString(string text, string salt = "")
        //{
        //    if (string.IsNullOrEmpty(text))
        //    {
        //        return string.Empty;
        //    }

        //    // Uses SHA256 to create the hash
        //    using (var sha = new System.Security.Cryptography.SHA256Managed())
        //    {
        //        // Convert the string to a byte array first, to be processed
        //        byte[] textBytes = Encoding.UTF8.GetBytes(text + salt);
        //        byte[] hashBytes = sha.ComputeHash(textBytes);

        //        // Convert back to a string, removing the '-' that BitConverter adds
        //        string hash = BitConverter
        //            .ToString(hashBytes)
        //            .Replace("-", string.Empty);

        //        return hash;
        //    }
        //}
        //public void GetUrlProtocol(string url)
        //{
        //    if (baseurl.ToLower().StartsWith("https://"))
        //    {
        //        protocol = "https";
        //    }
        //    else if (baseurl.ToLower().StartsWith("http://"))
        //    {
        //        protocol = "http";
        //    }
        //    else
        //    {
        //        protocol = "";
        //    }
        //}

        //public List<Urls> GetAllUrlsFromSite_v2()
        //{
        //    //Helper just cleans the urls
        //    CleanTable("urls");

        //    //Step 1 Add the base url to the database and mark it as visited                                    
        //    AddUrlToDb(Urls.Url, _VISITED);

        //    //Step 2 Get all the urls from the BasePage            
        //    //Step 3 Add the urls in the database
        //    AddUrlsToDb(GetLinks());

        //    UpdateControls();

        //    //Step 4 Select the next url that has not been visited 
        //    Urls = GetNextUrlFromDb();


        //    //Step 5 While there is an unvisited url
        //    while (Urls != null)
        //    {
        //        if (paused == false)
        //        {
        //            //Step 5.1 Initialize the new url
        //            //this.Urls.Url = FixUrl(this.Urls.Url);
        //            Urls.Url = GetAbsoluteUrlString(refererUrl, Urls.Url);


        //            if (!hasError)
        //            {
        //                //Step 5.2 Get the page
        //                VisitPage();
        //                //Step 5.3 Get the Title
        //                GetTitle();
        //            }

        //            hasError = false;
        //            //Step 5.4 Mark the visited url as visited
        //            UpdateUrlAsVisited(Urls.Id);
        //            //Step 5.5 Get all the unvisited link urls from the page and add them to the database
        //            refererUrl = Urls.Url;
        //            AddUrlsToDb(GetLinks());

        //            //Step 5.6 Get the next url from the database
        //            Urls = GetNextUrlFromDb();

        //            ////Step 5.7 Update the counters
        //            //UpdateControls();
        //            //this.form.SetlabelValue("Scraping on " + this.Urls.Url, calculateTotal().ToString(), calculateYetToVisit().ToString(), calculateVisited().ToString(), this.maxYetToVisit.ToString());

        //            //Step 5.7 go to step 5
        //        }
        //    }


        //    return GetAllDbUrls();
        //}
        //public List<Urls> GetAllDbUrls()
        //{
        //    //WebSiteScrapperContext context = new WebSiteScrapperContext();
        //    var url = Context.Urls.ToList();

        //    return url;
        //}
        //public void Pause()
        //{
        //    paused = true;
        //}
        //public void Start()
        //{
        //    paused = false;
        //}
    }
}
