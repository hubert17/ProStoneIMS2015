using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProStoneIMS2015.Models
{
    public class Vendor : TenantEntity
    {
        [Key]
        public int Id { get; set; }
        public string VendorName { get; set; }
    }
}