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

            string? pageAsString = GetPageAsString(uri);
            if (pageAsString == null)
            {
                return null;
            }
            else
            {
                HtmlDocument.LoadHtml(pageAsString);
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
                foreach (var element in HtmlDocument.DocumentNode.SelectNodes("//a"))
                {
                    if (element.Attributes.Contains("href"))
                    {
                        if (internalLinks == 1)
                        {
                            if (IsUrlInternal(Url, GetAbsoluteUrlString(Url, element.Attributes["href"].Value)))
                            {
                                Links.Add((new Tuple<string?, string?, string?>(Url, element.Attributes["href"].Value, GetAbsoluteUrlString(Url, element.Attributes["href"].Value))));
                            }
                        }
                        else if (internalLinks == 2)
                        {
                            if (!IsUrlInternal(Url, GetAbsoluteUrlString(Url, element.Attributes["href"].Value)))
                            {
                                Links.Add((new Tuple<string?, string?, string?>(Url, element.Attributes["href"].Value, GetAbsoluteUrlString(Url, element.Attributes["href"].Value))));
                            }
                        }
                        else
                        {
                            Links.Add((new Tuple<string?, string?, string?>(Url, element.Attributes["href"].Value, GetAbsoluteUrlString(Url, element.Attributes["href"].Value))));
                        }


                    }
                }
                return Links;
            }
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
        public string GetAbsoluteUrlString(string baseUrl, string url)
        {
            var uri = new Uri(url, UriKind.RelativeOrAbsolute);
            if (!uri.IsAbsoluteUri)
                uri = new Uri(new Uri(baseUrl), uri);
            return uri.ToString();
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
