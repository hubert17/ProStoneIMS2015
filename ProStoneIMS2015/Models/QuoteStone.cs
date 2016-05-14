using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProStoneIMS2015.Models
{
    public class QuoteStone : TenantEntity
    {
        public int Id { get; set; }
        public int? StoneId { get; set; }
        public double? PublishedPrice { get; set; }
        public double? Length { get; set; }
        public double? Width { get; set; }        
        public double? SquareFeet { get; set; } //SquareFeet: [Length]*[Width]/144
        public double? StateTax { get; set; }
        public double? Surcharge { get; set; }
        public int? NSlab { get; set; }
        public double? SquareFeetQty { get; set; }
        public double? NSlabSFdiv { get; set; } //Divide 60       
        public double? NSlabSF { get; set; } //NSlabSF: IIf(Not IsNull([NSlab]);[NSlab];Int([SquareFeetQty]/60)+1)        
        public double? SF { get { return SquareFeetQty; } set { value = SquareFeetQty; } }
        public double? PlusVal { get; set; } // 10 or 15?        
        public double? SFPlus { get; set; } //SFplus10: [SF]*1.1        
        public double? Price { get; set; } //Price: [PublishedPrice]+([PublishedPrice]*[Surcharge])       
        public double? TotPricePSF { get; set; } //TotPricePSF: [Price]+[Price]*[StateTax]       
        public double? SlabPrice { get; set; } //SlabPrice: [TotPricePSF]*[SquareFeet]        
        public double? Subtot { get; set; } //Subtot: [SlabPrice]*[NSlabSF]
        public double? FabMin { get; set; } // 25
        public double? FabMinPrice { get; set; } // 1.65        
        public double? FabPrice { get; set; } //FabPrice: IIf(Round([Price]*1.65;0)<25;25;Round([Price]*1.65;0))
        public double? FabPricePrintOveride { get; set; }        
        public double? FabPricePrintRounded { get; set; } //FabPricePrintTextbox: Round([FabPrice]+[TotPricePSF])        
        public double? FabPricePrint { get; set; } //FabPricePrint: IIf(IsNull([FabPricePrintOveride]);[FabPricePrintTextbox];[FabPricePrintOveride])        
        public double? TotSlabs { get; set; } //TotSlabs: [NSlabSF]*[SlabPrice]      
        public double? Total { get; set; } //Total: [NSlabSF]*[TotSlabs]      
        public double? SubtotFab { get; set; } //SubtotFab: [FabPrice]*[SFplus10]
        public double? SubtotFabPrint { get; set; } //SubtotFabPrint: [FabPricePrint]*[SFplus10]       
        public double? ExtAmt { get; set; } //ExtAmt: [PublishedPrice]*[SquareFeet]
        public int? EdgeId { get; set; }

        public int QuoteId { get; set; }
        [ForeignKey("QuoteId")]
        public virtual Quote Quote { get; set; }

    }
    public class QuoteMeasure : TenantEntity
    {
        public int Id { get; set; }
        public string MeasureName { get; set; }
        [Required]
        public double Length { get; set; }
        [Required]
        public double Width { get; set; }
        public int QuoteId { get; set; }
        [ForeignKey("QuoteId")]
        public virtual Quote Quote { get; set; }
    }

}