using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProStoneIMS2015.Models
{
    public class QuoteVM
    {
        public IEnumerable<SelectListItem> Edge { get; set; }
        public IEnumerable<SelectListItem> PayStatus { get; set; }
        public IEnumerable<SelectListItem> Backsplash { get; set; }
        public IEnumerable<SelectListItem> Lead { get; set; }
        public IEnumerable<SelectListItem> Salesman { get; set; }
        public IEnumerable<SelectListItem> Task { get; set; }
        public IEnumerable<SelectListItem> Asignedto { get; set; }
    }

    public class PayStatus
    {
        [Key]
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
    }

    public class Backsplash
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class TaskStatus
    {
        [Key]
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
    }

    public class PrintQuoteVM
    {
        public PrintQuoteVM()
        {
            Slab = new List<PrintQuoteDetail>();
            Sink = new List<PrintQuoteDetail>();
        }
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public string Address { get; set; }
        public string Address2 { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Notes { get; set; }
        public virtual List<PrintQuoteDetail> Slab { get; set; }
        public virtual List<PrintQuoteDetail> Sink { get; set; }
        public double? TotalCost { get; set; }
        public double TotalDeposit { get; set; }
        public double AmountDue { get; set; }

    }

    public class PrintQuoteDetail
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SlabNo { get; set; }
        public string SerialNo { get; set; }
        public string Catalog { get; set; }
        public double Quantity { get; set; }
    }


}