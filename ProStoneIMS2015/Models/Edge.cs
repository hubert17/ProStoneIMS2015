using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProStoneIMS2015.Models
{
    public class Edge : TenantEntity
    {
        [Key]
        public int Id { get; set; }
        public string EdgeName { get; set; }
        public double Price { get; set; }
        public string ImageFilename { get; set; }
        public int ServiceId { get; set; }

    }
}