using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProStoneIMS2015.Models
{
    public class Jservice : TenantEntity
    {
        [Key]
        public int Id { get; set; }
        public string ServiceName { get; set; }

        [Display(Name = "Quantity Mode")]
        public string ServiceCode { get; set; }
        public string ServiceCodeName { get; set; }
        public double Price { get; set; }
        public double WOPrice { get; set; }
        public bool EnableCustomerView { get; set; }
    }

    public class Jservice_type
    {
        [Key]
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Notes { get; set; }
        public bool Inactive { get; set; }        
    }
    public class JserviceVM
    {
        public Jservice jservice { get; set; }
        public IEnumerable<SelectListItem> Jservice_type { get; set; }
    }
}