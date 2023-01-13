using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WebSiteScrapper.Models
{
    public class Urls
    {

            [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
            [Key, Column(Order = 0)] 
            public Int64 Id { get; set; }
            public string? Title { get; set; }
            public string? Page { get; set; }            
            public string? Url { get; set; }
            public string? RefererUrl { get; set; }
            public string? Baseurl { get; set; }            
            public string? Hash { get; set; }            
            public DateTime Date { get; set; }            
            public  Boolean Visited { get; set; }            
            
        


    }
}
