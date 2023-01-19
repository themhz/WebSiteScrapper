using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSiteScrapper.Classes;
using WebSiteScrapper.Data;

using WebSiteScrapper;
using System.Configuration;
using Microsoft.EntityFrameworkCore;
using System.Security.Policy;
using WebSiteScrapper.Models;
using AngleSharp.Dom;

namespace WebSiteScrapperTests
{
    [TestClass()]
    public class HiveTester
    {

        private string appConfig = "Server=localhost;Database=WebSiteScrapper;User Id=NewSA;Password=Password@1234;TrustServerCertificate=True";



        [TestMethod()]
        public void TestAddUrlsToDb()
        {
            WebSiteScrapperContext _Context = new WebSiteScrapperContext(appConfig);
            Hive h = new Hive(_Context);
            SqlHandler sh = new SqlHandler(_Context);
            string url = "https://www.civiltech.gr/";
            Spider s = new Spider(url);
            sh.AddUrlsToDb(s.GetLinksAsTuples(),url, new Urls());
            Assert.IsTrue(sh.GetAllDbUrls().Count() > 0);
            sh.TruncateTable("Urls");

        }

        [TestMethod()]
        public void TestGetNextUrlFromDb()
        {
            WebSiteScrapperContext _Context = new WebSiteScrapperContext(appConfig);
            Hive h = new Hive(_Context);
            SqlHandler sh = new SqlHandler(_Context);
            string url = "https://www.civiltech.gr/";
            Spider s = new Spider(url);
            sh.AddUrlsToDb(s.GetLinksAsTuples(), url, new Urls());
            Assert.IsNotNull(sh.GetNextUrlFromDb());
            Assert.IsTrue(sh.GetNextUrlFromDb().Id == 1);
            sh.TruncateTable("Urls");

        }

        [TestMethod()]
        public void TestUpdateUrlAsVisited()
        {
            WebSiteScrapperContext _Context = new WebSiteScrapperContext(appConfig);
            Hive h = new Hive(_Context);
            SqlHandler sh = new SqlHandler(_Context);
            string url = "https://www.civiltech.gr/";
            sh.TruncateTable("Urls");
            Spider s = new Spider(url);
            sh.AddUrlsToDb(s.GetLinksAsTuples(), url, new Urls());
            Assert.IsNotNull(sh.GetNextUrlFromDb());
            sh.UpdateUrlAsVisited(sh.GetNextUrlFromDb().Id,url, new Urls());
            Assert.IsTrue(sh.GetNextUrlFromDb().Id == 2);

            sh.UpdateUrlAsVisited(sh.GetNextUrlFromDb().Id, url, new Urls());
            Assert.IsTrue(sh.GetNextUrlFromDb().Id == 3);
            sh.TruncateTable("Urls");

        }

        [TestMethod()]
        public void TestGetAllUrlsFrom()
        {
            WebSiteScrapperContext _Context = new WebSiteScrapperContext(appConfig);
            Hive h = new Hive(_Context);
            SqlHandler sh = new SqlHandler(_Context);
            string url = "https://www.civiltech.gr/";
            sh.TruncateTable("Urls");
            h.ScanWebSite("https://theotokatosfc.gr");
        }
    }
}
