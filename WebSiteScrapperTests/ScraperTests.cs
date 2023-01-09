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
using Microsoft.VisualBasic;

namespace WebSiteScrapper.Tests
{
    [TestClass()]
    public class ScraperTests
    {
        private string appConfig = "Server=localhost;Database=WebSiteScrapper;User Id=NewSA;Password=Password@1234;TrustServerCertificate=True";

        [TestMethod()]
        public void NewScraperObject()
        {
            WebSiteScrapperContext _Context = new WebSiteScrapperContext(appConfig);
            Scraper urls = new Scraper("https://theotokatosfc.gr", null, _Context);
            Assert.AreEqual("https://theotokatosfc.gr/", urls.Urls.Url);
            Assert.AreEqual("theotokatosfc &#8211; Σύλλογος Πυγμαχίας", urls.Urls.Title);
            Assert.AreEqual(0, urls.Urls.Id);
            Assert.AreEqual(null, urls.Urls.RefererUrl);
            Assert.AreEqual("https://theotokatosfc.gr/", urls.Urls.Baseurl);
            Assert.AreEqual("https://theotokatosfc.gr/", urls.baseurl);
            Assert.AreEqual(null, urls.form);

            //List<string> hashes = new List<string>();

            //for(int i = 0; i < 5; i++)
            //{
            //    hashes.Add(urls.HashString(urls.GetHtmlPage("https://theotokatosfc.gr").Text.Trim()));
            //}
            //string string1 = urls.GetHtmlPage("https://theotokatosfc.gr").Text.Trim();
            //string string2 = urls.GetHtmlPage("https://theotokatosfc.gr").Text.Trim();
            //string string3 = urls.GetHtmlPage("https://theotokatosfc.gr").Text.Trim();
            //string hash1 = urls.HashString(string1);
            //string hash2 = urls.HashString(string2);
            //string hash3 = urls.HashString(string3);
            //string test = urls.GetHtmlPage("https://theotokatosfc.gr").Text.Trim();
            //string test2 = test;

            //Assert.AreEqual(urls.HashString(test2), urls.HashString(test));
        }
        [TestMethod()]
        public void Test1IsUrlInternal_V2()
        {
            string url = "https://www.kipodomi-tools.gr";
            WebSiteScrapperContext _Context = new WebSiteScrapperContext(appConfig);
            Scraper urls = new Scraper(url, null, _Context);
            Assert.IsTrue(urls.IsUrlInternal_V2("https://www.kipodomi-tools.gr/el/shop/cart.html"));
        }
        [TestMethod()]
        public void Test2IsUrlInternal_V2()
        {
            string url = "https://www.kipodomi-tools.gr/";
            WebSiteScrapperContext _Context = new WebSiteScrapperContext(appConfig);
            Scraper urls = new Scraper(url, null, _Context);
            Assert.IsTrue(urls.IsUrlInternal_V2("https://www.kipodomi-tools.gr/el/shop/cart.html"));
        }

        [TestMethod()]
        public void Test2IsUrlInternal_V3()
        {
            string url = "http://www.kipodomi-tools.gr/";
            WebSiteScrapperContext _Context = new WebSiteScrapperContext(appConfig);
            Scraper urls = new Scraper(url, null, _Context);
            Assert.IsTrue(urls.IsUrlInternal_V2("https://www.kipodomi-tools.gr/el/shop/cart.html"));
        }

        [TestMethod()]
        public void Test2IsUrlInternal_V4()
        {
            string url = "https://www.kipodomi-tools.gr/";
            WebSiteScrapperContext _Context = new WebSiteScrapperContext(appConfig);
            Scraper urls = new Scraper(url, null, _Context);
            Assert.IsTrue(urls.IsUrlInternal_V2("http://www.kipodomi-tools.gr/el/shop/cart.html"));
        }

        [TestMethod()]
        public void Test2IsUrlInternal_V5()
        {
            string url = "https://www.kipodomi-tools.gr/";
            WebSiteScrapperContext _Context = new WebSiteScrapperContext(appConfig);
            Scraper urls = new Scraper(url, null, _Context);
            Assert.IsFalse(urls.IsUrlInternal_V2("cart.html"));
        }

        [TestMethod()]
        public void isUrlInternalTest()
        {
            string url = "https://theotokatosfc.gr";
            WebSiteScrapperContext _Context = new WebSiteScrapperContext(appConfig);
            Scraper urls = new Scraper(url, null, _Context);

            Assert.IsTrue(urls.IsUrlInternal(url));

            url = "theotokatosfc.gr";
            Assert.IsTrue(urls.IsUrlInternal(url));

            url = "http://facebook.theotokatosfc.gr";
            Assert.IsFalse(urls.IsUrlInternal(url));
        }

        [TestMethod()]
        public void isInDbTest()
        {
            WebSiteScrapperContext _Context = new WebSiteScrapperContext(appConfig);
            string url = "https://theotokatosfc.gr";
            Scraper urls = new Scraper(url, null, _Context);
            Assert.IsTrue(urls.IsInDb(url));


            url = "https://theotokatosfc.gr/about/";
            urls = new Scraper(url, null, _Context);
            Assert.IsTrue(urls.IsInDb(url));
        }
        [TestMethod()]
        public void isInDbTest2()
        {
            WebSiteScrapperContext _Context = new WebSiteScrapperContext(appConfig);
            string url = "https://theotokatosfc.gr/";
            Scraper urls = new Scraper(url, null, _Context);
            Assert.IsTrue(urls.IsInDb("https://theotokatosfc.gr"));


            url = "https://theotokatosfc.gr/about/";
            urls = new Scraper(url, null, _Context);
            Assert.IsTrue(urls.IsInDb(url));
        }

        [TestMethod()]
        public void isUrlInternal()
        {
            WebSiteScrapperContext _Context = new WebSiteScrapperContext(appConfig);
            string url = "https://theotokatosfc.gr/";
            Scraper urls = new Scraper(url, null, _Context);
            Assert.IsTrue(urls.IsUrlInternal(url));

        }
        [TestMethod()]
        public void isUrlInternal2()
        {
            WebSiteScrapperContext _Context = new WebSiteScrapperContext(appConfig);
            string url = "http://theotokatosfc.gr";
            Scraper urls = new Scraper(url, null, _Context);
            Assert.IsTrue(urls.IsUrlInternal(url));
        }

        [TestMethod()]
        public void isUrlInternal3()
        {
            WebSiteScrapperContext _Context = new WebSiteScrapperContext(appConfig);
            string url = "http://theotokatosfc.gr";
            Scraper urls = new Scraper(url, null, _Context);
            Assert.IsTrue(urls.IsUrlInternal("theotokatosfc.gr"));
        }

        [TestMethod()]
        public void isUrlInternal4()
        {
            WebSiteScrapperContext _Context = new WebSiteScrapperContext(appConfig);
            string url = "http://theotokatosfc.gr";
            Scraper urls = new Scraper(url, null, _Context);
            Assert.IsTrue(urls.IsUrlInternal("/"));
        }
        [TestMethod()]
        public void isUrlInternal5()
        {
            WebSiteScrapperContext _Context = new WebSiteScrapperContext(appConfig);
            string url = "http://theotokatosfc.gr";
            Scraper urls = new Scraper(url, null, _Context);
            Assert.IsFalse(urls.IsUrlInternal("https://facebook.com"));
        }
        [TestMethod]
        public void TestGetAbsoluteUrlString()
        {
            WebSiteScrapperContext _Context = new WebSiteScrapperContext(appConfig);

            Scraper urls = new Scraper("https://theotokatosfc.gr", null, _Context);
            Assert.AreEqual("https://theotokatosfc.gr/", urls.GetAbsoluteUrlString("https://theotokatosfc.gr",""));
        }

        //[TestMethod()]
        //public void fixUrl()
        //{
        //    WebSiteScrapperContext _Context = new WebSiteScrapperContext(appConfig);

        //    Scraper urls = new Scraper("https://theotokatosfc.gr", null, _Context);
        //    Assert.AreEqual("https://theotokatosfc.gr", urls.FixUrl("https://theotokatosfc.gr/"));
        //}

        //[TestMethod()]
        //public void fixUrl2()
        //{
        //    WebSiteScrapperContext _Context = new WebSiteScrapperContext(appConfig);            
        //    Scraper urls = new Scraper("https://theotokatosfc.gr/", null, _Context);
        //    Assert.AreEqual("https://theotokatosfc.gr", urls.FixUrl("https://theotokatosfc.gr/"));
        //}

        //[TestMethod()]
        //public void fixUrl3()
        //{
        //    WebSiteScrapperContext _Context = new WebSiteScrapperContext(appConfig);

        //    Scraper urls = new Scraper("https://theotokatosfc.gr/", null, _Context);
        //    Assert.AreEqual("https://theotokatosfc.gr", urls.FixUrl("https://theotokatosfc.gr"));
        //}

        //[TestMethod()]
        //public void fixUrl4()
        //{
        //    WebSiteScrapperContext _Context = new WebSiteScrapperContext(appConfig);            
        //    Scraper urls = new Scraper("https://theotokatosfc.gr", null, _Context);
        //    Assert.AreEqual("https://theotokatosfc.gr", urls.FixUrl("https://theotokatosfc.gr"));
        //}


        //[TestMethod()]
        //public void fixUrl5()
        //{
        //    WebSiteScrapperContext _Context = new WebSiteScrapperContext(appConfig);            
        //    Scraper urls = new Scraper("https://theotokatosfc.gr", null, _Context);
        //    Assert.AreEqual("https://theotokatosfc.gr/?add-to-cart=5826", urls.FixUrl("?add-to-cart=5826"));
        //}


        //[TestMethod()]
        //public void fixUrl6()
        //{
        //    WebSiteScrapperContext _Context = new WebSiteScrapperContext(appConfig);
        //    Scraper urls = new Scraper("https://theotokatosfc.gr", null, _Context);
        //    Assert.AreEqual("https://theotokatosfc.gr/programs/kids-karate-groups", urls.FixUrl("https://theotokatosfc.gr/programs/kids-karate-groups/"));
        //}

        //[TestMethod()]
        //public void fixUrl7()
        //{
        //    WebSiteScrapperContext _Context = new WebSiteScrapperContext(appConfig);
        //    Scraper urls = new Scraper("https://theotokatosfc.gr", null, _Context);
        //    Assert.AreEqual("https://theotokatosfc.gr", urls.FixUrl(" "));
        //    Assert.AreEqual("https://theotokatosfc.gr", urls.FixUrl("  "));
        //    Assert.AreEqual("https://theotokatosfc.gr", urls.FixUrl(""));
        //    Assert.AreEqual("https://theotokatosfc.gr", urls.FixUrl("   "));
        //}

        //[TestMethod()]
        //public void fixUrl8()
        //{
        //    WebSiteScrapperContext _Context = new WebSiteScrapperContext(appConfig);
        //    Scraper urls = new Scraper("https://theotokatosfc.gr", null, _Context);            
        //    Assert.AreEqual("https://theotokatosfc.gr/index.php/contacts", urls.FixUrl("index.php/contacts"));
        //}


    }
}