using ProStoneIMS2015.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Linq.Dynamic;
using System.Data.Entity;

namespace ProStoneIMS2015.Controllers
{
    [Authorize(Roles = "user, subscriber")]
    public class QuoteController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index(int? Id)
        {
            if (Id == null)
                return null;

            return View();
        }

        [HttpGet]
        public ActionResult Wizard()
        {
            var quote = new Quote();
            //ViewData["Stone_type"] = GetStoneType();
            ViewBag.PayStatus = new SelectList(db.PayStatuss
                .Select(s => new SelectListItem { Value = s.Id.ToString(), Text = s.Name }), "Value", "Text");
            ViewBag.Backsplash = new SelectList(db.Backsplashs
                .Select(s => new SelectListItem { Value = s.Id.ToString(), Text = s.Name }), "Value", "Text");
            ViewBag.Lead = new SelectList(db.Leads.Where(x => x.Inactive == false)
                .Select(s => new SelectListItem { Value = s.Id.ToString(), Text = s.LeadName }), "Value", "Text");
            ViewBag.Salesman = new SelectList(db.Salesmans.Where(x => x.Inactive == false)
                .Select(s => new SelectListItem { Value = s.Id.ToString(), Text = s.SalesmanName }), "Value", "Text");
            ViewBag.TaskStatus = new SelectList(db.TaskStatuss
                .Select(s => new SelectListItem { Value = s.Code, Text = s.Name }), "Value", "Text");
            ViewBag.AssignedTo = new SelectList(db.Assignedtos.Where(x => x.Inactive == false)
                .Select(s => new SelectListItem { Value = s.Id.ToString(), Text = s.AssignedtoName }), "Value", "Text");

            List<SelectListItem> Stones = new List<SelectListItem>();
            Stones.Add(new SelectListItem() { Text = "[My Hold Items]", Value = "-1" });
            Stones.AddRange(db.Stones.Where(x => x.Inactive == false)
                .Select(s => new SelectListItem { Value = s.Id.ToString(), Text = s.StoneName }));
            ViewBag.Stones = Stones;

            ViewBag.Edges = new SelectList(db.Edges.Where(x => x.Inactive == false)
                .Select(s => new SelectListItem { Value = s.Id.ToString(), Text = s.EdgeName }), "Value", "Text");

            List<SelectListItem> Sinks = new List<SelectListItem>();
            Sinks.Add(new SelectListItem() { Text = "[My Hold Items]", Value = "-1" });
            Sinks.AddRange(db.Sinks.Where(x => x.Inactive == false)
                .Select(s => new SelectListItem { Value = s.Id.ToString(), Text = s.SinkName }));
            ViewBag.Sinks = Sinks;

            return View("Wizard", quote);
        }

        [HttpPost]
        public string SubmitWizard(string jsonCustomer, string jsonMeasures, string jsonStones, string jsonSinks)
        {
            Quote quote = Newtonsoft.Json.JsonConvert.DeserializeObject<Quote>(jsonCustomer);
            List<QuoteMeasure> measures = Newtonsoft.Json.JsonConvert.DeserializeObject<List<QuoteMeasure>>(jsonMeasures);
            List<StoneInventory> stones = Newtonsoft.Json.JsonConvert.DeserializeObject<List<StoneInventory>>(jsonStones);
            List<SinkInventory> sinks = Newtonsoft.Json.JsonConvert.DeserializeObject<List<SinkInventory>>(jsonSinks);


            db.Quotes.Add(quote);

            if(quote.TaskStatus == "INS")
            {
                var inventoryToUpdate = db.StoneInventorys.Where(x => x.QuoteId == quote.Id);
                foreach (StoneInventory inv in inventoryToUpdate)
                {
                    inv.DateSold = quote.TaskDate;
                }
            }
            db.SaveChanges();

            foreach(var m in measures)
            {
                m.QuoteId = quote.Id;
            }
            db.QuoteMeasures.AddRange(measures);
            db.SaveChanges();

            foreach (var s in stones)
            {
                var inventoryToUpdate = db.StoneInventorys.Find(s.Id);
                inventoryToUpdate.QuoteId = quote.Id;
                inventoryToUpdate.EdgeId = s.EdgeId;
            }
            db.SaveChanges();

            //sinks.RemoveAll(s => s.SinkId < 1);
            foreach (var s in sinks)
            {
                var inventoryToUpdate = db.SinkInventorys.Find(s.Id);
                inventoryToUpdate.QuoteId = quote.Id;
            }
            db.SaveChanges();

            return Url.Action("Print", "Quote", new { id = quote.Id }, this.Request.Url.Scheme);
        }

        [HttpGet]
        public ActionResult EditWizard(int Id = 1)
        {
            var quote = db.Quotes.Find(Id);
            //ViewData["Stone_type"] = GetStoneType();
            ViewBag.PayStatus = new SelectList(db.PayStatuss
                .Select(s => new SelectListItem { Value = s.Id.ToString(), Text = s.Name }), "Value", "Text");
            ViewBag.Backsplash = new SelectList(db.Backsplashs
                .Select(s => new SelectListItem { Value = s.Id.ToString(), Text = s.Name }), "Value", "Text");
            ViewBag.Lead = new SelectList(db.Leads.Where(x => x.Inactive == false)
                .Select(s => new SelectListItem { Value = s.Id.ToString(), Text = s.LeadName }), "Value", "Text");
            ViewBag.Salesman = new SelectList(db.Salesmans.Where(x => x.Inactive == false)
                .Select(s => new SelectListItem { Value = s.Id.ToString(), Text = s.SalesmanName }), "Value", "Text");
            ViewBag.TaskStatus = new SelectList(db.TaskStatuss
                .Select(s => new SelectListItem { Value = s.Code, Text = s.Name }), "Value", "Text");
            ViewBag.AssignedTo = new SelectList(db.Assignedtos.Where(x => x.Inactive == false)
                .Select(s => new SelectListItem { Value = s.Id.ToString(), Text = s.AssignedtoName }), "Value", "Text");

            List<SelectListItem> Stones = new List<SelectListItem>();
            Stones.Add(new SelectListItem() { Text = "[My Hold Items]", Value = "-1" });
            Stones.AddRange(db.Stones.Where(x => x.Inactive == false)
                .Select(s => new SelectListItem { Value = s.Id.ToString(), Text = s.StoneName }));
            ViewBag.Stones = Stones;

            ViewBag.Edges = new SelectList(db.Edges.Where(x => x.Inactive == false)
                .Select(s => new SelectListItem { Value = s.Id.ToString(), Text = s.EdgeName }), "Value", "Text");

            List<SelectListItem> Sinks = new List<SelectListItem>();
            Sinks.Add(new SelectListItem() { Text = "[My Hold Items]", Value = "-1" });
            Sinks.AddRange(db.Sinks.Where(x => x.Inactive == false)
                .Select(s => new SelectListItem { Value = s.Id.ToString(), Text = s.SinkName }));
            ViewBag.Sinks = Sinks;

            return View(quote);
        }


        [HttpPost]
        public string EditWizardSubmit(string jsonCustomer)
        {
            Quote quote = Newtonsoft.Json.JsonConvert.DeserializeObject<Quote>(jsonCustomer);
            if (!ModelState.IsValid)
            {
                return "err";
            }

            try
            {
                db.Entry(quote).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch
            {
                return "err";
            }

            return Url.Action("Print", "Quote", new { id = quote.Id }, this.Request.Url.Scheme);
        }

        // GET: /Quote/Create
        [HttpGet]
        public ActionResult Create()
        {
            var quote = new QouteViewModel();
            //ViewData["Stone_type"] = GetStoneType();
            return PartialView("Create", quote);
        }

        // POST: /Quote/Create
        [HttpPost]
        public JsonResult Create(Quote quote)
        {
            if (ModelState.IsValid)
            {
                db.Quotes.Add(quote);
                db.SaveChanges();
                return Json(new
                {
                    success = true,
                    ctrl = "Quote",
                    action = "insquote",
                    RedirectUrl = Url.Action("Index", "Quote", new { id = quote.Id }),
                    id = quote.Id.ToString()
                });
            }
            return Json(quote, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Print(int Id)
        {
            PrintQuoteVM printQuote = new PrintQuoteVM();
            var quote = (from q in db.Quotes
                         where q.Id == Id
                         select new
                         {
                             Id = q.Id,
                             LastName = q.LastName,
                             FirstName = q.FirstName,
                             Address = q.Address,
                             City = q.City,
                             State = q.State,
                             Zip = q.Zip,
                             Phone = q.Phone,
                             Email = q.Email,
                             TotalCost = q.TotalCost
                         }).FirstOrDefault();

            printQuote.Id = quote.Id;
            printQuote.CustomerName = quote.FirstName + " " + quote.LastName;
            printQuote.Phone = quote.Phone;
            printQuote.Address = quote.Address;
            printQuote.Address2 = quote.City + ", " + quote.State + ", " + quote.Zip;
            printQuote.Email = quote.Email;

            var slabs = (from i in db.StoneInventorys
                        where i.QuoteId == Id
                       join s in db.Stones
                       on i.StoneId equals s.Id into si
                       from s in si.DefaultIfEmpty()
                       join e in db.Edges
                       on i.EdgeId equals e.Id
                       select new PrintQuoteDetail
                       {
                           Id = s.Id,
                           Name = s.StoneName + " " + s.Thickness,
                           SlabNo = i.SlabNo,
                           SerialNo = i.SerialNo,
                           Catalog = e.EdgeName,
                           //Quantity = Math.Round((double)(i.Length * i.Width) / 144.00, 2)
                       }).ToList();

            printQuote.Slab = slabs;            

            var sinks = (from i in db.SinkInventorys
                         where i.QuoteId == Id 
                        join s in db.Sinks
                        on i.SinkId equals s.Id into si
                        from s in si.DefaultIfEmpty()
                        select new PrintQuoteDetail
                        {
                            Id = s.Id,
                            Name = s.SinkName,
                            Catalog = s.CatalogID,
                            Quantity = 1
                        });

            //printQuote.Sink = sinks;
            printQuote.Sink = (sinks.GroupBy(x => new { x.Id, x.Name, x.Catalog, x.Quantity })
                                 .Select(x => new PrintQuoteDetail
                                 {
                                     Id = x.Key.Id,
                                     Name = x.Key.Name,
                                     Catalog = x.Key.Catalog,
                                     Quantity = x.Sum(z => z.Quantity)
                                 })).ToList();

            printQuote.TotalCost = quote.TotalCost;
            printQuote.TotalDeposit = 0.00;
            printQuote.AmountDue = 0.00;

            return View(printQuote);
        }
    }
}