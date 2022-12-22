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
    internal class WebSiteScrapperContext:DbContext
    {
        public DbSet<Urls> Urls { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(ConfigurationManager.ConnectionStrings["MyDbConnection"].ConnectionString);
        }
    }
}
