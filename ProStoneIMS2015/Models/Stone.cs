using Newtonsoft.Json;
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
    public class Stone : TenantEntity
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string StoneName { get; set; }
        public string Thickness { get; set; }
        public int Type { get; set; }
        public string ImageFilename { get; set; }

        [NotMapped]
        public string TypeName { get; set; }

        [NotMapped]
        public string StoneNameCm
        {
            get
            {
                return StoneName + " " + Thickness;
            }
        }

        public virtual ICollection<StoneInventory> StoneInventory { get; set; }
    }

    public class Stone_type : TenantEntity
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
    }

    public class StoneVM
    {
        public Stone stone { get; set; }
        public IEnumerable<SelectListItem> stone_type { get; set; }
    }

    public class StoneInventory : TenantEntity
    {
        public int Id { get; set; }
        public int StoneId { get; set; }
        [Display(Name = "Slab")]
        [ScriptIgnore]
        [ForeignKey("StoneId")]
        public virtual Stone Stone { get; set; }
        [NotMapped]
        public string StoneName { get; set; }
        [Display(Name = "Serial No")]
        public string SerialNo { get; set; }
        [Display(Name = "Slab No")]
        public string SlabNo { get; set; }
        public int? Length { get; set; }
        public int? Width { get; set; }
        [NotMapped]
        [Display(Name = "SF Quantity")]
        public double? SF { get; set; }
        public double? UnitPrice { get; set; }
        public double? SalesTax { get; set; }
        [NotMapped]
        public double? Total { get; set; }
        public string LotNo { get; set; }
        [Display(Name = "Vendor")]
        public int? VendorId { get; set; }
        [NotMapped]
        public string VendorName { get; set; }
        public bool Consignment { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? DateAdded { get; set; }
        public DateTime? DateSold { get; set; }
        [Display(Name = "Hold #")]
        public int? QuoteId { get; set; }
        [NotMapped]
        public string QuoteName { get; set; }
        public int? EdgeId { get; set; }
        [NotMapped]
        public string EdgeName { get; set; }

    }

    public class StoneInventoryList<T>
    {
        public List<T> Content { get; set; }

    }
}