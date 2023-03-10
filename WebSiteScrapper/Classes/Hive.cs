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
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace WebSiteScrapper.Classes
{
    public class Hive
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
        public const bool _NOTVISITED = false;

        public const int _ALL = 0;
        public const int _INTERNAL = 1;
        public const int _EXTERNAL = 2;
        public bool paused = false;


        public Hive(WebSiteScrapperContext _Context, Form1 _form)
        {
            Context = _Context;
            form = _form;
        }
        public Hive(WebSiteScrapperContext _Context)
        {
            Context = _Context;            
        }

        public void ScanWebSite(string url)
        {
            
            this.baseurl = url;
            this.AddUrlToDb(new Tuple<string?, string?, string?>(url, url, url), false);            
            Urls = GetNextUrlFromDb();

            while (Urls != null)
            {
                if (!this.paused)
                {
                    Spider s = new Spider(Urls.Url);
                    List<Tuple<string?, string?, string?>>? links = s.GetLinksAsTuples(_INTERNAL);

                    if (links != null)
                    {                                                
                        this.AddUrlsToDb(links);                        
                    }

                    Urls.Title = s.GetTitle();
                    Urls.Page = s.HtmlPage;
                    this.UpdateUrlAsVisited(Urls.Id);
                    Urls = GetNextUrlFromDb();
                }
                this.UpdateControls();
            }
        }
        /// <summary>
        /// This will add the urls to the database
        /// </summary>
        /// <param name="urls">BaseUrl,OriginalUrl,ParsedUrl</param>
        public void AddUrlsToDb(List<Tuple<string?, string?, string?>> urls)
        {
            if (urls != null)
            {
                foreach (var url in urls)
                {                    
                    AddUrlToDb(url, _NOTVISITED);                    
                }
            }
        }
        /// <summary>
        /// This will add the urls to the database
        /// </summary>
        /// <param name="urls">BaseUrl,OriginalUrl,ParsedUrl</param>
        public void AddUrlToDb(Tuple<string?, string?, string?> url, bool visited)
        {
            Context.ChangeTracker.Clear();
            
            

            Urls urls = new Urls();
            urls.Id = 0;
            urls.Baseurl = this.baseurl;
            urls.Url = url.Item3;
            urls.Date = DateTime.Now;
            urls.Hash = "";
            urls.Visited = visited;
            urls.RefererUrl = Urls==null? this.baseurl : Urls.Url;


            if (!IsInDb(url.Item3))
            {
                Context.Add(urls);
                Context.SaveChanges();
            }            
        }
        public Urls GetNextUrlFromDb()
        {
            //WebSiteScrapperContext context = new WebSiteScrapperContext();
            var url = Context.Urls.Where(s => s.Visited == false)
                        .FirstOrDefault();

            return url;
        }
        public void UpdateControls()
        {
            string message = "";
            if (Urls != null)
                message = "Scraping on " + Urls.Url;
            else
                message = "finished scraping all urls";

            form.SetlabelValue(message, CalculateTotal().ToString(), CalculateYetToVisit().ToString(), CalculateVisited().ToString(), maxYetToVisit.ToString());

            DataTable dt = form.CreateDataTable();
            List<Urls> urls = GetAllDbUrls();
            foreach (Urls url in urls)
            {
                DataRow row = dt.NewRow();
                //table.Columns.Add("Id", typeof(int));
                //table.Columns.Add("Title", typeof(string));
                //table.Columns.Add("OriginalUrl", typeof(string));
                //table.Columns.Add("Url", typeof(string));
                //table.Columns.Add("Baseurl", typeof(string));
                //table.Columns.Add("ReferenceUrl", typeof(string));
                //table.Columns.Add("visited", typeof(bool));
                row[0] = url.Id;
                row[1] = url.Title;
                row[2] = "";
                row[3] = url.Url;
                row[4] = url.Baseurl;
                row[5] = url.RefererUrl;
                row[6] = url.Visited;
                dt.Rows.Add(row);                
            }


            form.SetDataTable(dt);

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
        public void DeleteTableRows(string tableName)
        {
            string connectionString = Context.getConnectionString();
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
        public void TruncateTable(string tableName)
        {
            string connectionString = Context.getConnectionString();
            SqlConnection connection = new SqlConnection(connectionString);
            string sqlStatement = "Truncate table " + tableName;

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
        public bool IsInDb(string url)
        {
            Urls? lurl = Context.Urls.Where(s => s.Url == url).FirstOrDefault();
            return lurl != null ? true : false;
        }
        public void UpdateUrlAsVisited(long id)
        {
            Context.ChangeTracker.Clear();
            //Urls = Context.Urls.Find(id);

            Urls urls = Context.Urls.Find(id);
            if (urls != null)
            {
                urls.Baseurl = this.baseurl;                        
                urls.Hash = "";
                urls.Visited = _VISITED;
                urls.Page = Urls.Page;
                urls.Title = Urls.Title;
                                      
                Context.Update(urls);
                Context.SaveChanges();
            }

        }
        public string HashString(string text, string salt = "")
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            // Uses SHA256 to create the hash
            using (var sha = new System.Security.Cryptography.SHA256Managed())
            {
                // Convert the string to a byte array first, to be processed
                byte[] textBytes = Encoding.UTF8.GetBytes(text + salt);
                byte[] hashBytes = sha.ComputeHash(textBytes);

                // Convert back to a string, removing the '-' that BitConverter adds
                string hash = BitConverter
                    .ToString(hashBytes)
                    .Replace("-", string.Empty);

                return hash;
            }
        }
        //public List<Urls> GetAllUrlsFromSite_v2()
        //{
        //    //Helper just cleans the urls
        //    DeleteTableRows("urls");

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
        public List<Urls> GetAllDbUrls()
        {
            Context.ChangeTracker.Clear();
            var url = Context.Urls.ToList();
            

            return url;
        }
        public void Pause()
        {
            paused = true;
        }
        public void Start()
        {
            paused = false;
        }
    }
}
