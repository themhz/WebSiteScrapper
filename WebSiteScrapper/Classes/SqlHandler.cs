using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using WebSiteScrapper.Models;
using WebSiteScrapper.Data;
using System.ComponentModel;

namespace WebSiteScrapper.Classes
{
    public class SqlHandler
    {

        public int totalLinks = 0;
        public int linksVisited = 0;
        public int linksYetToVisit = 0;
        public int maxYetToVisit = 0;
        private string connectionString;
        public const bool _VISITED = true;
        public const bool _NOTVISITED = false;

        public const int _ALL = 0;
        public const int _INTERNAL = 1;
        public const int _EXTERNAL = 2;
        public WebSiteScrapperContext Context;

        public SqlHandler(WebSiteScrapperContext _Context)
        {
            Context = _Context;
            
        }

        public DataSet Select(string query)
        {
            // Create a connection
            using (SqlConnection connection = new SqlConnection(Context.getConnectionString()))
            {
                // Create a command
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // Open the connection
                    connection.Open();

                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataSet dataSet = new DataSet();
                    adapter.Fill(dataSet);

                    // Close the connection
                    connection.Close();

                    return dataSet;
                }
            }
        }

        public int Count(string query)
        {
            // Create a connection
            using (SqlConnection connection = new SqlConnection(Context.getConnectionString()))
            {
                // Create a command
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // Open the connection
                    connection.Open();

                    Int32 count = (Int32)command.ExecuteScalar();
                    connection.Close();

                    return count;
                }
            }
        }

        public void Insert(string query)
        {

        }

        public int CalculateVisited()
        {
            //WebSiteScrapperContext context = new WebSiteScrapperContext();
            //var url = Context.Urls.Where(s => s.Visited == true).ToList();

            return Count("select count(id) from Urls where Visited = 1");
        }
        public int CalculateYetToVisit()
        {
            //WebSiteScrapperContext context = new WebSiteScrapperContext();
            //var url = Context.Urls.Where(s => s.Visited == false).ToList();

            //if (maxYetToVisit < url.Count())
            //    maxYetToVisit = url.Count();

            //return url.Count();

            int count = Count("select count(id) from Urls where Visited = 0");
            if(maxYetToVisit < count)
               maxYetToVisit = count;

            return count;
        }
        public int CalculateTotal()
        {
            //WebSiteScrapperContext context = new WebSiteScrapperContext();
            //var url = Context.Urls.ToList();            
            return Count("select count(*) from Urls"); 
        }
        public void DeleteTableRows(string tableName)
        {
            string connectionString = Context.getConnectionString();
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
        public void TruncateTable(string tableName)
        {
            string connectionString = Context.getConnectionString();
            SqlConnection connection = new SqlConnection(connectionString);
            string sqlStatement = "Truncate table " + tableName;

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
        public bool IsInDb(string url)
        {
            Urls? lurl = Context.Urls.Where(s => s.OriginalUrl == url).FirstOrDefault();
            return lurl != null ? true : false;
        }
        public void UpdateUrlAsVisited(long id, string baseurl, Urls _urls)
        {
            Context.ChangeTracker.Clear();
            //Urls = Context.Urls.Find(id);

            Urls urls = Context.Urls.Find(id);
            if (urls != null)
            {
                urls.Baseurl = baseurl;
                urls.Hash = "";
                urls.Visited = _VISITED;
                //urls.Page = _urls.Page;
                urls.Title = _urls.Title;
                urls.Status = _urls.Status;
                urls.ContentType = _urls.ContentType;
                urls.CharacterSet = _urls.CharacterSet;
                urls.ContentEncoding = _urls.ContentEncoding;
                urls.Headers = _urls.Headers;
                urls.LastModified = _urls.LastModified;
                urls.Server = _urls.Server;
                urls.SupportsHeaders = _urls.SupportsHeaders;

                Context.Update(urls);
                Context.SaveChanges();
            }

        }

        /// <summary>
        /// This will add the urls to the database
        /// </summary>
        /// <param name="urls">BaseUrl,OriginalUrl,ParsedUrl</param>
        public void AddUrlsToDb(List<Tuple<string?, string?, string?>> urls, string baseurl, Urls _Urs)
        {
            if (urls != null)
            {
                foreach (var url in urls)
                {
                    AddUrlToDb(url, _NOTVISITED, baseurl, _Urs);
                }
            }
        }
        /// <summary>
        /// This will add the urls to the database
        /// </summary>
        /// <param name="urls">BaseUrl,OriginalUrl,ParsedUrl</param>
        public void AddUrlToDb(Tuple<string?, string?, string?> url, bool visited, string baseurl, Urls _Urs)
        {
            Context.ChangeTracker.Clear();



            Urls urls = new Urls();
            urls.Id = 0;
            urls.Baseurl = baseurl;
            urls.Url = url.Item3;
            urls.OriginalUrl = url.Item2;
            urls.Date = DateTime.Now;
            urls.Hash = "";
            urls.Visited = visited;
            urls.RefererUrl = _Urs == null ? baseurl : _Urs.Url;
            urls.Status = _Urs.Status;
            

            if (!IsInDb(url.Item3))
            {
                Context.Add(urls);
                Context.SaveChanges();
            }
        }
        public Urls GetNextUrlFromDb()
        {            
            var url = Context.Urls.Where(s => s.Visited == false)
                        .FirstOrDefault();
            return url;
        }

        public List<Urls> GetAllDbUrls()
        {            
            var url = Select("select * from Urls");
            return ConvertDataSetToUrls(url);
        }

        public Urls? GetUrlByUrlField(string url)
        {
            Urls urls = Context.Urls.Where(x => x.Url == url).First();
            return urls;
        }

        public List<Urls> ConvertDataSetToUrls(DataSet ds)
        {
            List<Urls> urls = new List<Urls>();
            if (ds.Tables.Count > 0)
            {                
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    Urls e = GetEntity<Urls>(dr);
                    urls.Add(e);
                }
            }
            
            return urls;
        }

        public static T GetEntity<T>(DataRow row) where T : new()
        {
            var entity = new T();
            var properties = typeof(T).GetProperties();

            foreach (var property in properties)
            {
                //Get the description attribute
                //var descriptionAttribute = (DescriptionAttribute)property.GetCustomAttributes(typeof(DescriptionAttribute), true).SingleOrDefault();
                //if (descriptionAttribute == null)
                //    continue;

                if (row[property.Name].ToString() != "")
                {
                    property.SetValue(entity, row[property.Name]);
                }

            }

            return entity;
        }

    }
}
