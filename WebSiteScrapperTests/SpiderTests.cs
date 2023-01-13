using Microsoft.VisualStudio.TestTools.UnitTesting;
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
using WebSiteScrapper.Classes;
using Microsoft.EntityFrameworkCore.Update;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;


namespace WebSiteScrapper.Tests
{
    [TestClass()]
    public class SpiderTests
    {
        private string appConfig = "Server=localhost;Database=WebSiteScrapper;User Id=NewSA;Password=Password@1234;TrustServerCertificate=True";
        
        #region TestGetHtmlPage

        [TestMethod()]
        public void TestGetHtmlPage_ReturnsSomething()
        {
            Spider s = new Spider();
            string? result = "";
            result = s.GetPageAsString("https://www.civiltech.gr");
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public void TestGetHtmlPage_LinkDoesntExist()
        {
            Spider s = new Spider();
            string? result = "";            
            result = s.GetPageAsString("https://www.civiltech.gr/adasdsadsa");            
            Assert.IsNull(result);

        }

        [TestMethod()]
        public void TestGetPageAsHtmlDocument_ReturnsDocument()
        {
            Spider s = new Spider();

            HtmlDocument? result = s.GetPageAsHtmlDocument("https://www.civiltech.gr");
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public void TestGetPageAsHtmlDocument_ReturnsNull()
        {
            Spider s = new Spider();
            HtmlDocument? result = s.GetPageAsHtmlDocument("https://www.civiltech.gr/adasdsadsa");
            Assert.IsNull(result);           
        }


        [TestMethod()]
        public void TestGetPageAsHtmlDocument_WrongDomain()
        {
            Spider s = new Spider();
            HtmlDocument? result = s.GetPageAsHtmlDocument("https://www.civiltech.gradasdsadsa");
            Assert.IsNull(result);

            result = s.GetPageAsHtmlDocument("https:/www.civiltech.gradasdsadsa");
            Assert.IsNull(result);


            result = s.GetPageAsHtmlDocument("httpwww.civiltech.gradasdsadsa");
            Assert.IsNull(result);
        }


        #endregion
        #region TestGetTitle
        [TestMethod()]
        public void GetTitle_Constructor_IsNotNull()
        {
            Spider s = new Spider("https://www.civiltech.gr");
            string? title = s.GetTitle();
            Assert.IsNotNull(title);
        }

        [TestMethod()]
        public void GetTitle_IsNull()
        {
            Spider s = new Spider();
            Assert.IsNull(s.GetTitle());
        }

        [TestMethod()]
        public void GetTitle_GetPageAsHtmlDocument_IsNotNull()
        {
            Spider s = new Spider();
            var page = s.GetPageAsHtmlDocument("https://www.civiltech.gr");
            Assert.IsNotNull(s.GetTitle());
        }

        #endregion
        #region TestGetUrls

        [TestMethod()]
        public void GetLinksAsTuples_IsNull()
        {
            Spider s = new Spider("https://www.civiltech.gr/asdsadsa");
            List< Tuple<string?, string?, string?>>? links = s.GetLinksAsTuples();            
            Assert.IsNull(links);
            s = new Spider("httsadsadsawww.civiltech.gr/asdsadsa");
            links = s.GetLinksAsTuples();
            Assert.IsNull(links);
        }


        [TestMethod()]
        public void GetLinksAsTuples_IsNotNull()
        {
            Spider s = new Spider("https://www.civiltech.gr");
            List<Tuple<string?, string?, string?>>? links = s.GetLinksAsTuples();
            Assert.IsNotNull(links);           
        }


        [TestMethod()]
        public void GetLinksAsTuples_InternalOnly()
        {
            Spider s = new Spider("https://www.civiltech.gr");
            List<Tuple<string?, string?, string?>>? links = s.GetLinksAsTuples(1);
            
            foreach(Tuple<string?, string?, string?> t in links)
            {
                Assert.IsTrue(s.IsUrlInternal(s.Url,t.Item3));
            }
            
        }

        [TestMethod()]
        public void GetLinksAsTuples_ExternalOnly()
        {
            Spider s = new Spider("https://www.civiltech.gr");
            List<Tuple<string?, string?, string?>>? links = s.GetLinksAsTuples(2);

            foreach (Tuple<string?, string?, string?> t in links)
            {
                Assert.IsTrue(!s.IsUrlInternal(s.Url, t.Item3));
            }

        }

        [TestMethod()]
        public void GetLinksAsTuples_All()
        {
            Spider s = new Spider("https://www.civiltech.gr");
            List<Tuple<string?, string?, string?>>? links = s.GetLinksAsTuples(0);

            foreach (Tuple<string?, string?, string?> t in links)
            {
                if (s.IsUrlInternal(s.Url, t.Item3))
                {
                    Assert.IsTrue(s.IsUrlInternal(s.Url, t.Item3));
                }
                else
                {
                    Assert.IsFalse(s.IsUrlInternal(s.Url, t.Item3));
                }
                
            }

        }

        #endregion
        #region TestGetImages

        [TestMethod()]
        public void GetImagesAsTuples_IsNull()
        {
            Spider s = new Spider("https://www.civiltech.gr/asdsadsa");
            List<Tuple<string?, string?, string?>>? images = s.GetLinksAsTuples();
            Assert.IsNull(images);
            s = new Spider("httsadsadsawww.civiltech.gr/asdsadsa");
            images = s.GetImagesAsTuples();
            Assert.IsNull(images);
        }


        [TestMethod()]
        public void GetImagesAsTuples_IsNotNull()
        {
            Spider s = new Spider("https://www.civiltech.gr");
            List<Tuple<string?, string?, string?>>? images = s.GetImagesAsTuples();
            Assert.IsNotNull(images);
        }

        #endregion

        #region TestIsUrlInternal            
            [TestMethod()]
            public void IsUrlInternal_OnDomain_True()
            {
                Spider s = new Spider("https://www.civiltech.gr");
                bool isInternal = s.IsUrlInternal("https://www.civiltech.gr", "dfdsfdsf");
                Assert.IsTrue(isInternal);
            }

            [TestMethod()]
            public void IsUrlInternal_OnSubDomain_True()
            {
                Spider s = new Spider("https://www.civiltech.gr/asdsadsa");
                bool isInternal = s.IsUrlInternal("https://www.civiltech.gr", "dfdsfdsf");
                Assert.IsTrue(isInternal);
            }

            [TestMethod()]
            public void IsUrlInternal_OnEmptySubDomain_True()
            {
                Spider s = new Spider("https://www.civiltech.gr");
                bool isInternal = s.IsUrlInternal("https://www.civiltech.gr", "");
                Assert.IsTrue(isInternal);
            }

            [TestMethod()]
            public void IsUrlInternal_OnBackSlash_True()
            {
                Spider s = new Spider("https://www.civiltech.gr");
                bool isInternal = s.IsUrlInternal("https://www.civiltech.gr", "/");
                Assert.IsTrue(isInternal);
            }
            [TestMethod()]
            public void IsUrlInternal_OnOtherSite_True()
            {
                Spider s = new Spider("https://www.civiltech.gr");
                bool isInternal = s.IsUrlInternal("https://www.civiltech.gr", "http://facebook.com");
                Assert.IsFalse(isInternal);

                isInternal = s.IsUrlInternal("https://www.civiltech.gr", "https://facebook.com");
                Assert.IsFalse(isInternal);
            }
            #endregion

    }
}