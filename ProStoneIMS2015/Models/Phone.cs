using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProStoneIMS2015.Models
{
    public class Phone : TenantEntity
    {
        [Display(Name = "ID")]
        [Key]
        public int PhoneId { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string Model { get; set; }

        [Required]
        [Display(Name = "Description")]
        public string Company { get; set; }

        [Required]
        [Display(Name = "Price")]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

    }
}