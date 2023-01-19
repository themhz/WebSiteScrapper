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
using AngleSharp.Dom;

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
        public bool hasError = false;
        public WebSiteScrapperContext Context;
        public const bool _VISITED = true;
        public const bool _NOTVISITED = false;

        public const int _ALL = 0;
        public const int _INTERNAL = 1;
        public const int _EXTERNAL = 2;
        public bool paused = false;
        public SqlHandler sqlh;

        public Hive(WebSiteScrapperContext _Context, Form1 _form)
        {
            Context = _Context;
            form = _form;
            sqlh = new SqlHandler(_Context);

        }
        public Hive(WebSiteScrapperContext _Context)
        {
            Context = _Context;            
        }

        public void ScanWebSite(string url)
        {
            Spider s = new Spider(url);
            this.form.Status = "Initializing " + url;
            this.Urls = new Urls();
            this.Urls.Url = url;
            this.baseurl = s.CheckRedirection(this.Urls, url);

            sqlh.AddUrlToDb(new Tuple<string?, string?, string?>(this.baseurl, this.baseurl, this.baseurl), false, this.baseurl, Urls);            
            Urls = sqlh.GetNextUrlFromDb();

            while (Urls != null)
            {
                if (!this.paused)
                {                                        
                    s = new Spider(Urls.Url);                    
                    List<Tuple<string?, string?, string?>>? links = new List<Tuple<string?, string?, string?>>();
                    List<Tuple<string?, string?, string?>>? _links = s.GetLinksAsTuples(_INTERNAL);                    
                    
                    if(_links != null)
                    {
                        s.CashedRedirectedUrl = "";
                        foreach (Tuple<string?, string?, string?> link in _links)                        
                        {
                            if (!links.Contains(link) && !sqlh.IsInDb(link.Item2))
                            {                                
                                this.form.Status = "Checking " + Urls.Url + " with " + link.Item2;
                                Tuple<string?, string?, string?>  link2 = new Tuple<string?, string?, string?>(link.Item1, link.Item2, s.CheckRedirection(Urls, link.Item2));
                                links.Add(link2);                                
                            }
                        }


                        if (links != null)
                        {
                            sqlh.AddUrlsToDb(links, this.baseurl, Urls);
                        }
                    }
                    


                    Urls.Title = s.GetTitle();
                    Urls.Page = s.HtmlPage;
                    sqlh.UpdateUrlAsVisited(Urls.Id, this.baseurl, Urls);
                    Urls = sqlh.GetNextUrlFromDb();
                }

                string message = "";
                if (Urls != null)
                    message = "Crawling on " + Urls.Url;
                else
                    message = "finished crawling all urls";

                this.form.Message = message;
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
