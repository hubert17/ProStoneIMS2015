using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace ProStoneIMS2015.Models
{
    public class Sink : TenantEntity
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string SinkName { get; set; }
        public string SinkMiniName { get; set; }
        public string CatalogID { get; set; }
        [Required]
        public double Price { get; set; }
        public string ImageFilename { get; set; }
        public int Type { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public string TypeName { get; set; }
    }
    public class Sink_type : TenantEntity
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
    }
    public class SinkVM
    {
        public Sink sink { get; set; }
        public IEnumerable<SelectListItem> sink_type { get; set; }
    }

    public class SinkInventory : TenantEntity
    {
        public int Id { get; set; }
        public int SinkId { get; set; }
        [Display(Name = "Sink")]
        [ScriptIgnore]
        [ForeignKey("SinkId")]
        public virtual Sink Sink { get; set; }
        [NotMapped]
        public string SinkName { get; set; }
        public string SerialNo { get; set; }
        [Display(Name = "Vendor")]
        public int? VendorId { get; set; }
        [NotMapped]
        public string VendorName { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime? DateAdded { get; set; }
        public double? UnitPrice { get; set; }
        public double? SalesTax { get; set; }
        [NotMapped]
        public double? Total { get; set; }
        public DateTime? DateSold { get; set; }
        [Display(Name = "Hold #")]
        public int? QuoteId { get; set; }
        [NotMapped]
        public string QuoteName { get; set; }
    }

}