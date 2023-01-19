using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using HtmlAgilityPack;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;
using WebSiteScrapper.Data;
using WebSiteScrapper.Models;
using Azure;
using System.Xml;
using System.Security.Policy;
using AngleSharp.Common;
using System.Diagnostics;

namespace WebSiteScrapper.Classes
{
    public class Spider
    {
        public string? Url;
        public string? HtmlPage;
        public string? Title;
        public List<Tuple<string?, string?, string?>>? Links; //BaseUrl,OriginalUrl,ParsedUrl
        public List<Tuple<string?, string?, string?>>? Images; //BaseUrl,OriginalUrl,ParsedUrl        
        public HtmlDocument HtmlDocument;
        public Spider()
        {
            this.HtmlDocument = new HtmlDocument();
        }
        public Spider(string url)
        {
            this.Url = url;
            this.HtmlDocument = new HtmlDocument();
            this.GetPageAsHtmlDocument(url);
        }
        public HtmlDocument? GetPageAsHtmlDocument(string uri)
        {

            HtmlPage = GetPageAsString(uri);
            if (HtmlPage == null)
            {
                return null;
            }
            else
            {
                HtmlDocument.LoadHtml(HtmlPage);
            }


            return HtmlDocument;
        }
        public List<Tuple<string?, string?, string?>>? GetLinksAsTuples(int internalLinks = 0)
        {
            this.Links = new List<Tuple<string?, string?, string?>>();
            if (HtmlDocument == null)
            {
                return null;
            }
            else if (HtmlDocument != null && HtmlDocument.DocumentNode.SelectNodes("//a") == null)
            {
                return null;
            }
            else
            {
                HtmlNodeCollection _nc = filtedNoed(HtmlDocument.DocumentNode.SelectNodes("//a"));
                foreach (var element in _nc)
                {
                    
                    if (element.Attributes.Contains("href") && !element.Attributes["href"].Value.StartsWith("#"))
                    {
                        string href = element.Attributes["href"].Value;
                        //string absoluturl = this.CheckRedirection(this.Url, href);                        
                        //string absoluturl = null;
                        var link = new Tuple<string?, string?, string?>(Url, href, href);                        
                        if (internalLinks == 1)
                        {
                            if (IsUrlInternal(Url, href))
                            {
                                Links.Add(link);
                            }
                        }
                        else if (internalLinks == 2)
                        {
                            if (!IsUrlInternal(Url, href))
                            {
                                Links.Add(link);
                            }
                        }
                        else
                        {
                            Links.Add(link);
                        }

                    }
                }
                return Links;
            }
        }

        public HtmlNodeCollection filtedNoed(HtmlNodeCollection _nc)
        {
            HtmlNodeCollection nc = new HtmlNodeCollection(null);
            List<string> values = new List<string>();

            foreach (var node in _nc)
            {
                if (node.Attributes.Contains("href") == true && !values.Contains(node.Attributes["href"].Value))
                {
                    values.Add(node.Attributes["href"].Value);
                    nc.Add(node);
                }
                
            }

            return nc;
        }
        public List<Tuple<string?, string?, string?>>? GetImagesAsTuples()
        {
            this.Links = new List<Tuple<string?, string?, string?>>();
            if (HtmlDocument == null)
            {
                return null;
            }
            else if (HtmlDocument != null && HtmlDocument.DocumentNode.SelectNodes("//img") == null)
            {
                return null;
            }
            else
            {
                foreach (var element in HtmlDocument.DocumentNode.SelectNodes("//img"))
                {
                    if (element.Attributes.Contains("src"))
                    {
                        Links.Add((new Tuple<string?, string?, string?>(Url, element.Attributes["src"].Value, GetAbsoluteUrlString(Url, element.Attributes["src"].Value))));
                    }
                }
                return Links;
            }
        }
        public string? GetPageAsString(string uri)
        {
            string? response = Task.Run(async () =>
            {
                var client = new HttpClient();
                try
                {
                    var response = await client.GetAsync(uri);
                    if (response.IsSuccessStatusCode)
                    {
                        return await response.Content.ReadAsStringAsync();
                    }
                    return null;
                }
                catch (Exception ex)
                {
                    return null;
                }

            }).GetAwaiter().GetResult();
            return response;
        }
        public string? GetTitle()
        {
            if (this.Title != null)
            {
                return this.Title;
            }
            else if (this.HtmlDocument != null && this.HtmlDocument.DocumentNode.SelectSingleNode("//title") != null)
            {
                this.Title = this.HtmlDocument.DocumentNode.SelectSingleNode("//title").InnerText.ToString();
            }

            return this.Title;
        }        
        public string GetAbsoluteUrlString(string baseUrl, string _url)
        {
          
            var url = new Uri(new Uri(baseUrl), _url);
            return url.ToString();
        }

        public string CashedRedirectedUrl = "";
        public string CheckRedirection(Urls urls, string _url)
        {
            string redirectedUrl = "";

            if (CashedRedirectedUrl == "")
            {                
                Uri myUri = new Uri(urls.Url);

                // Create a 'HttpWebRequest' object for the specified url.
                //Thread.Sleep(500);
                HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create(myUri);

                // Send the request and wait for response.
                HttpWebResponse myHttpWebResponse;
                try
                {
                    myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();
                    urls.Status = myHttpWebResponse.StatusCode.ToString();
                    urls.ContentType = myHttpWebResponse.ContentType;
                    urls.CharacterSet = myHttpWebResponse.CharacterSet;
                    urls.ContentEncoding = myHttpWebResponse.ContentEncoding.ToString();
                    urls.Headers = myHttpWebResponse.Headers.ToString();
                    urls.LastModified = myHttpWebResponse.LastModified.ToString();
                    urls.Server = myHttpWebResponse.Server.ToString();
                    urls.SupportsHeaders = myHttpWebResponse.SupportsHeaders.ToString();

                    if (myHttpWebResponse.StatusCode == HttpStatusCode.OK)
                    {

                        redirectedUrl = myHttpWebResponse.ResponseUri.AbsoluteUri.ToString();
                    }
                    else
                    {
                        redirectedUrl = myHttpWebResponse.ResponseUri.AbsoluteUri.ToString();
                    }

                    CashedRedirectedUrl = redirectedUrl;
                    myHttpWebResponse.Close();
                }
                catch (Exception ex)
                {
                    urls.Status = ex.Message;
                }
            }
               
                      
            
            if(CashedRedirectedUrl != "")
            {
                return (new Uri(new Uri(CashedRedirectedUrl), _url)).ToString();
            }
            else
            {
                return "";
            }
            

        }
        public bool IsUrlInternal(string? baseUrl, string? url)
        {
            bool match = false;
            try
            {
                var uri = new Uri(this.GetAbsoluteUrlString(baseUrl, url));
                var uri2 = new Uri(baseUrl);
                match = uri.Host.Equals(uri2.Host, StringComparison.InvariantCultureIgnoreCase);
            }
            catch (Exception ex)
            {

            }



            return match;
        }
    }
}
