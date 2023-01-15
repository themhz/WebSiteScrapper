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

            Spider s = new Spider("https://www.civiltech.gr/");
            h.AddUrlsToDb(s.GetLinksAsTuples());
            Assert.IsTrue(h.GetAllDbUrls().Count() > 0);
            h.TruncateTable("Urls");

        }

        [TestMethod()]
        public void TestGetNextUrlFromDb()
        {
            WebSiteScrapperContext _Context = new WebSiteScrapperContext(appConfig);
            Hive h = new Hive(_Context);

            Spider s = new Spider("https://www.civiltech.gr/");
            h.AddUrlsToDb(s.GetLinksAsTuples());
            Assert.IsNotNull(h.GetNextUrlFromDb());
            Assert.IsTrue(h.GetNextUrlFromDb().Id == 1);
            h.TruncateTable("Urls");

        }

        [TestMethod()]
        public void TestUpdateUrlAsVisited()
        {
            WebSiteScrapperContext _Context = new WebSiteScrapperContext(appConfig);
            Hive h = new Hive(_Context);
            h.TruncateTable("Urls");
            Spider s = new Spider("https://www.civiltech.gr/");
            h.AddUrlsToDb(s.GetLinksAsTuples());
            Assert.IsNotNull(h.GetNextUrlFromDb());
            h.UpdateUrlAsVisited(h.GetNextUrlFromDb().Id);
            Assert.IsTrue(h.GetNextUrlFromDb().Id == 2);

            h.UpdateUrlAsVisited(h.GetNextUrlFromDb().Id);
            Assert.IsTrue(h.GetNextUrlFromDb().Id == 3);
            h.TruncateTable("Urls");

        }

        [TestMethod()]
        public void TestGetAllUrlsFrom()
        {
            WebSiteScrapperContext _Context = new WebSiteScrapperContext(appConfig);
            Hive h = new Hive(_Context);
            h.TruncateTable("Urls");
            h.ScanWebSite("https://theotokatosfc.gr");
        }
    }
}
