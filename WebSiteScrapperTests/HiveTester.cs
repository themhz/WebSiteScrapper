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
using static System.Net.Mime.MediaTypeNames;

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
            sh.AddUrlsToDb(s.GetLinksAsTuples(), url, new Urls());
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
            sh.UpdateUrlAsVisited(sh.GetNextUrlFromDb().Id, url, new Urls());
            Assert.IsTrue(sh.GetNextUrlFromDb().Id == 2);

            sh.UpdateUrlAsVisited(sh.GetNextUrlFromDb().Id, url, new Urls());
            Assert.IsTrue(sh.GetNextUrlFromDb().Id == 3);
            sh.TruncateTable("Urls");

        }

        //[TestMethod()]
        //public void TestGetAllUrlsFrom()
        //{
        //    WebSiteScrapperContext _Context = new WebSiteScrapperContext(appConfig);
        //    Hive h = new Hive(_Context);
        //    SqlHandler sh = new SqlHandler(_Context);
        //    string url = "https://www.civiltech.gr/";
        //    sh.TruncateTable("Urls");
        //    h.ScanWebSite("https://theotokatosfc.gr");
        //}


        [TestMethod()]
        public void TestCompareStringsAreSame()
        {
            StringManager sm = new StringManager();
            var test = 100*sm.CompareStrings("ena", "ena");
            Assert.AreEqual(test, 100);
            //CompareStrings(string s1, string s2)
        }


        [TestMethod()]
        public void TestCompareStringsAreNotSame()
        {
            Spider s = new Spider();
            //string page1 = s.GetPageAsString("http://kipodomi-tools.gr");
            //string page2 = s.GetPageAsString("http://kipodomi-tools.gr");

            StringManager sm = new StringManager();
            var test = 100 * sm.CompareStrings("enaa", "ena");
            Assert.AreNotEqual(test, 100);
            //CompareStrings(string s1, string s2)
        }


        [TestMethod()]
        public void TestCompareStringsAreSimilarGreateThan90Percent()
        {
            Spider s = new Spider();
            string page1 = s.GetPageAsString("http://kipodomi-tools.gr");
            string page2 = s.GetPageAsString("http://kipodomi-tools.gr");

            StringManager sm = new StringManager();
            var test = 100 * sm.CompareStrings(page1, page2);
            Assert.IsTrue(test > 98);            
        }


        [TestMethod()]
        public void TestGetStringDifferenceShowDifference()
        {
            Spider s = new Spider();
            string page1 = s.GetPageAsString("http://kipodomi-tools.gr");
            string page2 = s.GetPageAsString("http://kipodomi-tools.gr");

            StringManager sm = new StringManager();
            var test = sm.GetStringDifference(page1, page2);

            
            //Assert.IsTrue(test > 98);
        }

        [TestMethod()]
        public void TestCalculateStringDifference()
        {
            Spider s = new Spider();
            string page1 = s.GetPageAsString("http://kipodomi-tools.gr");
            string page2 = s.GetPageAsString("http://kipodomi-tools.gr");

            StringManager sm = new StringManager();
            var test = 100 * sm.CalculateStringDifference(page1, page2);

            Assert.IsTrue(test > 98);
        }


        [TestMethod()]
        public void TestCompareTextFiles()
        {
            Spider s = new Spider();
            string page1 = s.GetPageAsString("http://kipodomi-tools.gr");
            string page2 = s.GetPageAsString("http://kipodomi-tools.gr");

            string tempFilePath = Path.GetTempFileName();
            using (StreamWriter writer = new StreamWriter(tempFilePath))
            {
                writer.Write(page1);
            }

            string tempFilePath2 = Path.GetTempFileName();
            using (StreamWriter writer = new StreamWriter(tempFilePath2))
            {
                writer.Write(page2);
            }

            StringManager sm = new StringManager();
            var test = sm.CompareTextFiles(tempFilePath, tempFilePath2);

            
        }

    }
}
