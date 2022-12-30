using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebSiteScrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSiteScrapper.Data;
using System.Configuration;

namespace WebSiteScrapper.Tests
{
    [TestClass()]
    public class ScraperTests
    {
        private string appConfig = "Server=localhost;Database=WebSiteScrapper;User Id=NewSA;Password=Password@1234;TrustServerCertificate=True";
        [TestMethod()]
        public void isUrlInternalTest()
        {
            string url = "https://theotokatosfc.gr";           
            WebSiteScrapperContext _Context = new WebSiteScrapperContext(appConfig);
            Scraper urls = new Scraper(url, null, _Context);

            Assert.IsTrue(urls.isUrlInternal(url));

            url = "theotokatosfc.gr";
            Assert.IsTrue(urls.isUrlInternal(url));

            url = "http://facebook.theotokatosfc.gr";
            Assert.IsFalse(urls.isUrlInternal(url));
        }

        [TestMethod()]
        public void isInDbTest()
        {
            
            
            WebSiteScrapperContext _Context = new WebSiteScrapperContext(appConfig);
            string url = "https://theotokatosfc.gr/";
            Scraper urls = new Scraper(url, null, _Context);
            Assert.IsTrue(urls.isInDb(url));


            url = "https://theotokatosfc.gr/about/";
            urls = new Scraper(url, null, _Context);
            Assert.IsTrue(urls.isInDb(url));
        }
    }
}