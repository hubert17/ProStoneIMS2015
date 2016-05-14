using Multitenant.Interception.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProStoneIMS2015.Models
{
    public class Subscriber
    {

        [Key]
        public int TenantId { get; set; }

        [Index(IsUnique = true)]
        [Remote("IsKeyExists", "Subscriber", ErrorMessage = "This key is already in use. Please try another one.")]
        [Required(ErrorMessage = "Subscriber Key is required. Please input your desired combination of letters and digits.")]
        [MaxLength(6, ErrorMessage = "Subscriber Key must be of up to 6 (six) characters long only.")]
        [StringLength(6, ErrorMessage = "Subscriber Key must be of 6 (six) characters long.", MinimumLength = 6)]
        [RegularExpression(@"\S*(\S*([a-zA-Z]\S*[0-9])|([0-9]\S*[a-zA-Z]))\S*", ErrorMessage = "Must have at least one letter and digit")]
        [Display(Name = "Subscriber Key")]
        public string SubscriberKey { get; set; }

        [Required(ErrorMessage = "Please fill your Company Name.")]
        [Display(Name = "Company Name")]
        public string CompanyName { get; set; }

        [Required(ErrorMessage = "Please fill your First Name.")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Please fill your Last Name.")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Name")]
        public string RegName { get { return FirstName + " " + LastName; } }

        public string Street { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Country { get; set; }

        [Display(Name = "Complete Address")]
        public string CompleteAddress { get { return Street + ", " + Address + ", " + City + ", " + State + ", " + Zip + ", " + Country; } }

        [Display(Name = "Company Phone")]
        public string Phone { get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "Primary Email is required.")]
        [Display(Name = "Primary Email")]
        public string Email { get; set; }

        [EmailAddress]
        [Display(Name = "Alternate Email")]
        public string AltEmail { get; set; }

        [EmailAddress]
        [Display(Name = "Customer Contact Email")]
        public string CustomerContactEmail { get; set; }

        [Display(Name = "Customer Contact Number")]
        public string CustomerContactPhone { get; set; }

        [Display(Name = "Subcriber Since")]
        public DateTime? MembershipDate { get; set; }

        public bool Inactive { get; set; }

        //Company default
        public double? StateTax { get; set; }
        public double? PlusVal { get; set; }
        public double? FabMin { get; set; } // 25
        public double? FabMinPrice { get; set; } // 1.65  
        public string DefaultState { get; set; }
    }

    public class SubscriberViewModel : Subscriber
    {
        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

}