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

namespace WebSiteScrapper
{
    public class Scraper
    {
        private readonly string baseurl;
        private string? url;
        private string? title;
        private string? hashedPage;
        private List<string> urls;
        private Form1 form;
        

        public Scraper(string url, Form1 _form)
        {
            //Initialize the base url
            this.baseurl = url;
            this.url = url;
            this.urls = new List<string>();
            this.urls.Add(url);
            this.form = _form;
            
        }
        public HtmlDocument Scrape()
        {
            var htmlDoc = new HtmlDocument();
            using (var client = new WebClient())
            {
                try
                {
                    var html = client.DownloadString(url);
                    this.hashedPage = this.HashString(html);
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

        private string getTitle(HtmlDocument doc)
        {            
            if (doc.DocumentNode.SelectSingleNode("//title") != null)
            {
                return doc.DocumentNode.SelectSingleNode("//title").InnerText.ToString();
            }
            else
            {
                return "";
            }            
        }

        private List<HtmlNode>? getLinks(HtmlDocument doc)
        {
           
            if (doc.DocumentNode.SelectNodes("//a") == null)
            {
                return null;
            }
            else
            {
                var links = new List<HtmlNode>();
                foreach (var element in doc.DocumentNode.SelectNodes("//a"))
                {
                    if (element.Attributes.Contains("href"))
                    {
                        links.Add(element);
                    }
                    
                }
                return links;
            }                                     
        }
        
        //Get all the urls from the website
        public List<string> GetAllUrlsFromSite()
        {
            this.cleanTable("urls");
            int counter = 1;
            addUrlToDab(this.url, counter);
            
            for (int i = 0; i < urls.Count; i++)
            {
                
                this.url = urls[i];                
                HtmlDocument doc = this.Scrape();
                this.title = getTitle(doc);
                List <HtmlNode>? links = getLinks(doc);

                this.form.SetlabelValue("Working on " + this.url);                

                if (links != null)
                {
                    foreach (HtmlNode element in links)
                    {

                        //if they are not in the list add them
                        if (!this.urls.Contains(element.Attributes["href"].Value))
                        {
                            string tmpUrl = element.Attributes["href"].Value;

                            if(tmpUrl != null)
                            {
                                if (!(tmpUrl.StartsWith("http://") || tmpUrl.StartsWith("https://")) && (tmpUrl.StartsWith("/") || tmpUrl.StartsWith("\"")))
                                {
                                    tmpUrl = this.baseurl + tmpUrl.Substring(1, tmpUrl.Length - 1);
                                    if (!this.urls.Contains(tmpUrl))
                                    {
                                        this.urls.Add(tmpUrl);
                                        counter++;
                                        addUrlToDab(tmpUrl, counter);
                                    }
                                }
                                else
                                {
                                    if (tmpUrl.StartsWith(baseurl) || tmpUrl.StartsWith(baseurl.Replace("http", "https")))
                                    {
                                        if (!this.urls.Contains(tmpUrl))
                                        {
                                            this.urls.Add(tmpUrl);
                                            counter++;
                                            addUrlToDab(tmpUrl, counter++);
                                        }
                                    }
                                }
                            }
                            
                        }
                    }
                }                                             
            }
            this.form.SetlabelValue("Finished");
            return this.urls;
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

        private void addUrlToDab(string tmpUrl, int metritis)
        {
            WebSiteScrapperContext context = new WebSiteScrapperContext();
            Urls url = new Urls()
            {
                Id = Guid.NewGuid().ToString(),
                Title = this.title,
                Baseurl = this.url,
                Url = tmpUrl,
                Date = new DateTime(),
                Hash = HashString(tmpUrl),
                Metritis = metritis

            };

            if (url != null)
            {
                context.Add(url);
                context.SaveChanges();
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
