using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebSiteScrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSiteScrapper.Data;
using System.Configuration;
using Microsoft.EntityFrameworkCore;
using System.Security.Policy;

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

        [TestMethod()]
        public void isUrlInternal()
        {
            WebSiteScrapperContext _Context = new WebSiteScrapperContext(appConfig);
            string url = "https://theotokatosfc.gr/";
            Scraper urls = new Scraper(url, null, _Context);
            Assert.IsTrue(urls.isUrlInternal(url));

        }
        [TestMethod()]
        public void isUrlInternal2()
        {
            WebSiteScrapperContext _Context = new WebSiteScrapperContext(appConfig);
            string url = "https://theotokatosfc.gr";
            Scraper urls = new Scraper(url, null, _Context);
            Assert.IsTrue(urls.isUrlInternal(url));
        }


        [TestMethod()]
        public void fixUrl()
        {
            WebSiteScrapperContext _Context = new WebSiteScrapperContext(appConfig);
            
            Scraper urls = new Scraper("https://theotokatosfc.gr", null, _Context);
            Assert.AreEqual("https://theotokatosfc.gr", urls.fixUrl("https://theotokatosfc.gr/"));
        }

        [TestMethod()]
        public void fixUrl2()
        {
            WebSiteScrapperContext _Context = new WebSiteScrapperContext(appConfig);            
            Scraper urls = new Scraper("https://theotokatosfc.gr/", null, _Context);
            Assert.AreEqual("https://theotokatosfc.gr", urls.fixUrl("https://theotokatosfc.gr/"));
        }

        [TestMethod()]
        public void fixUrl3()
        {
            WebSiteScrapperContext _Context = new WebSiteScrapperContext(appConfig);
            
            Scraper urls = new Scraper("https://theotokatosfc.gr/", null, _Context);
            Assert.AreEqual("https://theotokatosfc.gr", urls.fixUrl("https://theotokatosfc.gr"));
        }

        [TestMethod()]
        public void fixUrl4()
        {
            WebSiteScrapperContext _Context = new WebSiteScrapperContext(appConfig);            
            Scraper urls = new Scraper("https://theotokatosfc.gr", null, _Context);
            Assert.AreEqual("https://theotokatosfc.gr", urls.fixUrl("https://theotokatosfc.gr"));
        }


        [TestMethod()]
        public void fixUrl5()
        {
            WebSiteScrapperContext _Context = new WebSiteScrapperContext(appConfig);            
            Scraper urls = new Scraper("https://theotokatosfc.gr", null, _Context);
            Assert.AreEqual("https://theotokatosfc.gr/?add-to-cart=5826", urls.fixUrl("?add-to-cart=5826"));
        }


        [TestMethod()]
        public void fixUrl6()
        {
            WebSiteScrapperContext _Context = new WebSiteScrapperContext(appConfig);
            Scraper urls = new Scraper("https://theotokatosfc.gr", null, _Context);
            Assert.AreEqual("https://theotokatosfc.gr/programs/kids-karate-groups", urls.fixUrl("https://theotokatosfc.gr/programs/kids-karate-groups/"));
        }

        [TestMethod()]
        public void fixUrl7()
        {
            WebSiteScrapperContext _Context = new WebSiteScrapperContext(appConfig);
            Scraper urls = new Scraper("https://theotokatosfc.gr", null, _Context);
            Assert.AreEqual("https://theotokatosfc.gr", urls.fixUrl(" "));
            Assert.AreEqual("https://theotokatosfc.gr", urls.fixUrl("  "));
            Assert.AreEqual("https://theotokatosfc.gr", urls.fixUrl(""));
            Assert.AreEqual("https://theotokatosfc.gr", urls.fixUrl("   "));
        }

        [TestMethod()]
        public void fixUrl8()
        {
            WebSiteScrapperContext _Context = new WebSiteScrapperContext(appConfig);
            Scraper urls = new Scraper("https://theotokatosfc.gr", null, _Context);            
            Assert.AreEqual("https://theotokatosfc.gr/index.php/contacts", urls.fixUrl("index.php/contacts"));
        }
    }
}