using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSiteScrapper.Models;
using System.Configuration;

namespace WebSiteScrapper.Data
{
    public class WebSiteScrapperContext:DbContext
    {        
        internal DbSet<Urls> Urls { get; set; } = null!;
        private string ConnectionString;
        public WebSiteScrapperContext(string _connectionString){
            this.ConnectionString = _connectionString;
         
        }        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {         
            optionsBuilder.UseSqlServer(this.ConnectionString);
        }

        public string getConnectionString()
        {
            return this.ConnectionString;
        }
    }
}