using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProStoneIMS2015.Models
{
    public class Lead : TenantEntity
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string LeadName { get; set; }
    }
}