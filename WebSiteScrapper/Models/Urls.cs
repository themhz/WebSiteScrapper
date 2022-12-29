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
    internal class Urls
    {

            [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
            [Key, Column(Order = 0)] 
            public Int64 Id { get; set; }

            public string? Title { get; set; }
            [Required]
            public string? Url { get; set; }

            [Required]            
            public string? Baseurl { get; set; }

            [Required]
            public string? Hash { get; set; }

            [Required]
            public DateTime? Date { get; set; }

            [Required]
            public  Boolean Visited { get; set; }

            
    }
}
