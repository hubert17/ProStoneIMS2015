using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProStoneIMS2015.Models
{
    public class Quote : TenantEntity
    {
        public Quote()
        {
                                                                                                                                                                        
        }

        [Key]
        [Display(Name = "Quote No.")]
        public int Id { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        [Display(Name = "User Job No.")]
        public int? JobNo { get; set; }
        public bool Verified { get; set; }
        public int? Backsplash { get; set; }
        public string PayStatus { get; set; }
        public int? Lead { get; set; }
        public DateTime? DateCreated { get; set; }
        public int? Salesman { get; set; }
        public string TaskStatus { get; set; }
        public DateTime? TaskDate { get; set; }
        public string TaskTime { get; set; }
        public int? AssignedTo { get; set; }
        public double? FabPrice { get; set; }
        public  double? TotalCost { get; set; }

        public virtual ICollection<QuoteHistory> Historys { get; set; }
        public virtual ICollection<QuoteStone> Stones { get; set; }
        public virtual ICollection<QuoteSink> Sinks { get; set; }
        public virtual ICollection<QuoteService> Services { get; set; }
    }

    public class QouteViewModel
    {
        [Display(Name = "Quote No.")]
        public int Id { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string State { get; set; }
        [Required]
        public string Zip { get; set; }
        [Required]
        public string Phone { get; set; }
        [Required]
        public string Email { get; set; }


    }
    public class QuoteHistory : TenantEntity
    {
        public int Id { get; set; }
        public string TaskStatus { get; set; }
        public DateTime? TaskDate { get; set; }
        public string TaskTime { get; set; }
        public int? AssignedToId { get; set; }
        public bool? Rendered { get; set; }
        public int QuoteId { get; set; }
        [ForeignKey("QuoteId")]
        public virtual Quote Quote { get; set; }

    }
    public class QuoteSink : TenantEntity
    {
        public int Id { get; set; }
        public int SinkId { get; set; }
        public double? Quantity { get; set; }
        public double? Price { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public double? Amount { get; private set; }
        public int QuoteId { get; set; }
        [ForeignKey("QuoteId")]
        public virtual Quote Quote { get; set; }
    }
    public class QuoteService : TenantEntity
    {
        public int Id { get; set; }
        [Required]
        public int ServiceId { get; set; }
        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public string ServiceCode { get; set; }
        public double? Quantity { get; set; }
        public double? Price { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public double? Amount { get; private set; }
        public int QuoteId { get; set; }
        [ForeignKey("QuoteId")]
        public virtual Quote Quote { get; set; }
    }

    public class QuotePayment : TenantEntity
    {
        public int Id { get; set; }
        [Required]
        public DateTime PaymentDate { get; set; }
        public double? Amount { get; set; }
        public int QuoteId { get; set; }
        [ForeignKey("QuoteId")]
        public virtual Quote Quote { get; set; }
    }

}