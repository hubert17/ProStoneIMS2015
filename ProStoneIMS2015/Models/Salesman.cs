using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProStoneIMS2015.Models
{
    public class Salesman : TenantEntity
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string SalesmanName { get; set; }
        public string Email { get; set; }
        public string ContactNo { get; set; }

    }
}