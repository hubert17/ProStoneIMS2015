using ProStoneIMS2015.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Linq.Dynamic;
using System.Data.Entity;
using System.Text;

namespace ProStoneIMS2015.Controllers
{
    [NoCache]
    public class SinkinventoryController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Home
        public ActionResult Index()
        {
            var sinkLookup = db.Sinks.Select(s => s.SinkName);
            var vendorLookup = db.Vendors.Select(s => s.VendorName);
            ViewBag.Typeahead = string.Join("','", sinkLookup.ToArray());
            ViewBag.Typeahead += "','Consignment','" + string.Join("','", vendorLookup.ToArray());

            return View();
        }

        public JsonResult SinkLookup()
        {
            var rec = db.Sinks.ToArray().Select(s => s.SinkName);
            return Json(new { rec }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetInventory(int page = 1, int limit = 5, string sortBy = "Id", string direction = "desc", string searchString = null, bool showInactive = false)
        {
            var records = new List<SinkInventory>();
            int total = 0;

            var qry = from s in db.SinkInventorys
                      join t in db.Sinks
                      on s.SinkId equals t.Id into st
                      from t in st.DefaultIfEmpty()
                      join v in db.Vendors
                      on s.VendorId equals v.Id into sv
                      from v in sv.DefaultIfEmpty()
                      join q in db.Quotes
                      on s.QuoteId equals q.Id into sq
                      from q in sq.DefaultIfEmpty()
                      select new
                      {
                          Id = s.Id,
                          SinkId = s.SinkId,
                          SinkName = t.SinkName,
                          SerialNo = s.SerialNo,
                          VendorName = v.VendorName,
                          DateAdded = s.DateAdded,
                          UnitPrice = s.UnitPrice,
                          SalesTax = s.SalesTax,
                          Total = s.UnitPrice + (s.UnitPrice * s.SalesTax),
                          QuoteId = s.QuoteId,
                          QuoteName = s.QuoteId == null ? String.Empty : s.Id + " - " + q.FirstName + " " + q.LastName,
                          DateSold = s.DateSold,
                          Inactive = s.Inactive
                      };

            if (showInactive)
            {
                records = qry
                            .Where(x => searchString == null || x.SinkName.Contains(searchString) || x.VendorName.Contains(searchString))
                            .OrderBy(sortBy + " " + direction)
                            .Skip((page - 1) * limit)
                            .Take(limit)
                            .ToList()
                                .Select(s => new SinkInventory
                                {
                                    Id = s.Id,
                                    SinkId = s.SinkId,
                                    SinkName = s.SinkName,
                                    SerialNo = s.SerialNo,
                                    VendorName = s.VendorName,
                                    DateAdded = s.DateAdded,
                                    UnitPrice = s.UnitPrice,
                                    SalesTax = s.SalesTax,
                                    Total = s.Total,
                                    QuoteName = s.QuoteName,
                                    QuoteId = s.QuoteId,
                                    DateSold = s.DateSold,
                                    Inactive = s.Inactive
                                }).ToList();
                ViewBag.showInactive = false;
                ViewBag.labelInactive = "Hide";
                total = qry.Where(x => searchString == null || x.SinkName.Contains(searchString)).Count();
            }
            else
            {
                records = qry
                            .Where(x => (x.Inactive == false) && searchString == null || x.SinkName.Contains(searchString) || x.VendorName.Contains(searchString))
                            .OrderBy(sortBy + " " + direction)
                            .Skip((page - 1) * limit)
                            .Take(limit)
                            .ToList()
                                .Select(s => new SinkInventory
                                {
                                    Id = s.Id,
                                    SinkId = s.SinkId,
                                    SinkName = s.SinkName,
                                    VendorName = s.VendorName,
                                    SerialNo = s.SerialNo,
                                    DateAdded = s.DateAdded,
                                    UnitPrice = s.UnitPrice,
                                    SalesTax = s.SalesTax,
                                    Total = s.Total,
                                    QuoteId = s.QuoteId,
                                    QuoteName = s.QuoteName,
                                    DateSold = s.DateSold,
                                    Inactive = s.Inactive
                                }).ToList();
                ViewBag.showInactive = true;
                ViewBag.labelInactive = "Show";
                total = qry.Where(x => (x.Inactive == false) && searchString == null || x.SinkName.Contains(searchString)).Count();
            }

            // Count

            return Json(new { records, total }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.SinkLookup = new SelectList(db.Sinks.Where(x => x.Inactive == false)
                .Select(s => new SelectListItem { Value = s.Id.ToString(), Text = s.SinkName }), "Value", "Text");

            ViewBag.VendorLookup = new SelectList(db.Vendors.Where(x => x.Inactive == false)
                .Select(s => new SelectListItem { Value = s.Id.ToString(), Text = s.VendorName }), "Value", "Text");

            var inventory = new SinkInventory();
            return PartialView("Create", inventory);
        }

        [HttpPost]
        public JsonResult Create(SinkInventory inventory)
        {
            if (ModelState.IsValid)
            {
                inventory.SinkName = System.Text.RegularExpressions.Regex.Replace(inventory.SinkName.Trim().ToLower(), @"\s+", " ");
                int SinkIdFromDb = db.Sinks.Where(x => x.SinkName.Trim().ToLower() == inventory.SinkName).Select(p => p.Id).DefaultIfEmpty(-1).Single();
                if (SinkIdFromDb > 0)
                    inventory.SinkId = SinkIdFromDb;
                else
                {
                    var sink = new Sink();
                    System.Globalization.TextInfo textInfo = new System.Globalization.CultureInfo("en-US", false).TextInfo;
                    sink.SinkName = textInfo.ToTitleCase(inventory.SinkName);
                    db.Sinks.Add(sink);
                    db.SaveChanges();
                    inventory.SinkId = sink.Id;
                }

                int Count = int.Parse(Request["txtSinkCount"]);
                int MaxSinkId = db.SinkInventorys.Where(x => x.SinkId == inventory.SinkId).Select(p=>p.Id).DefaultIfEmpty(1).Max();
                for (int i = 0; i < Count; i++)
                {
                    var clone = new SinkInventory();                    
                    clone.SinkId = inventory.SinkId;
                    clone.SerialNo = (i + MaxSinkId).ToString("D2");
                    clone.VendorId = inventory.VendorId;
                    clone.DateAdded = inventory.DateAdded;
                    clone.UnitPrice = inventory.UnitPrice;
                    clone.SalesTax = inventory.SalesTax;
                    db.SinkInventorys.Add(clone);
                }

                db.SaveChanges();
                return Json(new
                {
                    success = true,
                    ctrl = "Inventory",
                    action = "insert",
                    id = inventory.Id.ToString()
                    //,col = new[] {
                    //    db.Sinks.Where(x => x.Id == inventory.StoneId).Select(s => s.StoneName).FirstOrDefault(),
                    //    //inventory.SerialNo,
                    //    Request["txtSlabCount"].ToString(),
                    //    inventory.Length.ToString(),
                    //    inventory.Width.ToString(),
                    //    inventory.LotNo,
                    //    db.Vendors.Where(x => x.Id == inventory.VendorId).Select(s => s.VendorName).FirstOrDefault(),
                    //    inventory.Consignment.ToString(),
                    //    inventory.QuoteId == null ? null : inventory.Id + " - " + db.Quotes.Where(x => x.Id == inventory.QuoteId).Select(s => s.FirstName + " " + s.LastName).FirstOrDefault()
                    //}
                });
            }
            return Json(inventory, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Edit(int id = 0)
        {

            ViewBag.SinkLookup = new SelectList(db.Sinks.Where(x => x.Inactive == false)
                .Select(s => new SelectListItem { Value = s.Id.ToString(), Text = s.SinkName }), "Value", "Text");

            ViewBag.VendorLookup = new SelectList(db.Vendors.Where(x => x.Inactive == false)
                .Select(s => new SelectListItem { Value = s.Id.ToString(), Text = s.VendorName }), "Value", "Text");

            var inventory = db.SinkInventorys.Find(id);

            if (inventory == null)
            {
                return HttpNotFound();
            }
            
            return PartialView("Edit", inventory);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(SinkInventory inventory)
        {
            if (ModelState.IsValid)
            {
                db.Entry(inventory).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new
                {
                    success = true,
                    action = "edit",
                    inactive = inventory.Inactive.ToString(),
                    col = new[] {
                        //inventory.Id.ToString(),
                        db.Sinks.Where(x => x.Id == inventory.SinkId).Select(s => s.SinkName).FirstOrDefault(),
                        inventory.SerialNo,
                        db.Vendors.Where(x => x.Id == inventory.VendorId).Select(s => s.VendorName).FirstOrDefault(),
                        String.Format("{0:MM/dd/yyyy}",inventory.DateAdded),
                        inventory.UnitPrice.ToString(),
                        inventory.SalesTax.ToString(),
                        (inventory.UnitPrice + (inventory.UnitPrice * inventory.SalesTax)).ToString(),               
                        inventory.QuoteId == null ? null : inventory.Id + " - " + db.Quotes.Where(x => x.Id == inventory.QuoteId).Select(s => s.FirstName + " " + s.LastName).FirstOrDefault(),
                        String.Format("{0:MM/dd/yyyy}",inventory.DateSold)
                    }
                });
            }
            return PartialView("Edit", inventory);
        }

        public ActionResult Delete(int id = 0)
        {
            SinkInventory inventory = db.SinkInventorys.Find(id);
            if (inventory == null)
            {
                return HttpNotFound();
            }

            inventory.SinkName = db.Sinks.Where(x => x.Id == inventory.SinkId).Select(s => s.SinkName).FirstOrDefault();
            return PartialView("Delete", inventory);
        }


        //
        // POST: /Stone/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var inventory = db.SinkInventorys.Find(id);
            db.SinkInventorys.Remove(inventory);
            db.SaveChanges();
            return Json(new { success = true, action = "delete" });
        }

    }


    //    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    //public sealed class NoCacheAttribute : ActionFilterAttribute
    //{
    //    public override void OnResultExecuting(ResultExecutingContext filterContext)
    //    {
    //        filterContext.HttpContext.Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
    //        filterContext.HttpContext.Response.Cache.SetValidUntilExpires(false);
    //        filterContext.HttpContext.Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
    //        filterContext.HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
    //        filterContext.HttpContext.Response.Cache.SetNoStore();

    //        base.OnResultExecuting(filterContext);
    //    }
    //}

}