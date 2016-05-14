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
    public class InventoryController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Home
        public ActionResult Index()
        {
            var stoneLookup = db.Stones.Select(s => s.StoneName);
            var vendorLookup = db.Vendors.Select(s => s.VendorName);
            ViewBag.Typeahead = string.Join("','", stoneLookup.ToArray());
            ViewBag.Typeahead += "','Consignment','" + string.Join("','", vendorLookup.ToArray());

            return View();
        }

        public JsonResult StoneLookup()
        {
            var rec = db.Stones.ToArray().Select(s => s.StoneName);
            return Json(new { rec }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetInventory(int page = 1, int limit = 5, string sortBy = "Id", string direction = "desc", string searchString = null, bool showInactive = false)
        {
            var records = new List<StoneInventory>();
            int total = 0;

            var qry = from s in db.StoneInventorys
                      join t in db.Stones
                      on s.StoneId equals t.Id into st
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
                          StoneId = s.StoneId,
                          StoneName = t.StoneName + " " + t.Thickness,
                          SlabNo = String.IsNullOrEmpty(s.SlabNo) ? "###" : s.SlabNo,
                          SerialNo = String.IsNullOrEmpty(s.SerialNo) ? "###" : s.SerialNo,
                          Length = s.Length,
                          Width = s.Width,
                          SF = Math.Round((double) (s.Length * s.Width) / 144.00, 2),
                          UnitPrice = s.UnitPrice,
                          SalesTax = s.SalesTax,
                          Total = (s.UnitPrice + s.UnitPrice * ((s.SalesTax/100) ?? 0)) * ((double)(s.Length * s.Width) / 144.00),
                          LotNo = s.LotNo,
                          VendorName = v.VendorName,
                          Consignment = s.Consignment,
                          QuoteId = s.QuoteId,
                          QuoteName = s.QuoteId == null ? String.Empty : s.Id + " - " + q.FirstName + " " + q.LastName,
                          DateAdded = s.DateAdded,
                          DateSold = s.DateSold,
                          Inactive = s.Inactive
                      };

            if (!String.IsNullOrEmpty(searchString) && searchString.ToLower().Contains("consignment"))
            {
                qry = qry.Where(x => x.Consignment == true);
                searchString = null;
            }

            if (showInactive)
            {
                records = qry
                            .Where(x => searchString == null || x.StoneName.Contains(searchString) || x.VendorName.Contains(searchString))
                            .OrderBy(sortBy + " " + direction)
                            .Skip((page - 1) * limit)
                            .Take(limit)
                            .ToList()
                                .Select(s => new StoneInventory
                                {
                                    Id = s.Id,
                                    StoneId = s.StoneId,
                                    StoneName = s.StoneName,
                                    SlabNo = s.SlabNo,
                                    SerialNo = s.SerialNo,
                                    Length = s.Length,
                                    Width = s.Width,
                                    SF = s.SF,
                                    UnitPrice = s.UnitPrice,
                                    SalesTax = s.SalesTax,
                                    Total = s.Total,
                                    LotNo = s.LotNo,
                                    VendorName = s.VendorName,
                                    Consignment = s.Consignment,
                                    QuoteName = s.QuoteName,
                                    QuoteId = s.QuoteId,
                                    DateAdded = s.DateAdded,
                                    DateSold = s.DateSold,
                                    Inactive = s.Inactive
                                }).ToList();
                ViewBag.showInactive = false;
                ViewBag.labelInactive = "Hide";
                total = qry.Where(x => searchString == null || x.StoneName.Contains(searchString)).Count();
            }
            else
            {
                records = qry
                            .Where(x => (x.Inactive == false) && searchString == null || x.StoneName.Contains(searchString) || x.VendorName.Contains(searchString))
                            .OrderBy(sortBy + " " + direction)
                            .Skip((page - 1) * limit)
                            .Take(limit)
                            .ToList()
                                .Select(s => new StoneInventory
                                {
                                    Id = s.Id,
                                    StoneId = s.StoneId,
                                    StoneName = s.StoneName,
                                    SlabNo = s.SlabNo,
                                    SerialNo = s.SerialNo,
                                    Length = s.Length,
                                    Width = s.Width,
                                    SF = s.SF,
                                    UnitPrice = s.UnitPrice,
                                    SalesTax = s.SalesTax,
                                    Total = s.Total,
                                    LotNo = s.LotNo,
                                    VendorName = s.VendorName,
                                    Consignment = s.Consignment,
                                    QuoteId = s.QuoteId,
                                    QuoteName = s.QuoteName,
                                    DateAdded = s.DateAdded,
                                    DateSold = s.DateSold,
                                    Inactive = s.Inactive
                                }).ToList();
                ViewBag.showInactive = true;
                ViewBag.labelInactive = "Show";
                total = qry.Where(x => (x.Inactive == false) && searchString == null || x.StoneName.Contains(searchString)).Count();
            }

            // Count

            return Json(new { records, total }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.StoneLookup = new SelectList(db.Stones.Where(x => x.Inactive == false)
                .Select(s => new SelectListItem { Value = s.Id.ToString(), Text = s.StoneName + " " + s.Thickness }), "Value", "Text");

            ViewBag.VendorLookup = new SelectList(db.Vendors.Where(x => x.Inactive == false)
                .Select(s => new SelectListItem { Value = s.Id.ToString(), Text = s.VendorName }), "Value", "Text");

            var inventory = new StoneInventory();
            return PartialView("Create", inventory);
        }

        [HttpPost]
        public JsonResult Create(StoneInventory inventory)
        {
            if (ModelState.IsValid)
            {
                inventory.StoneName = System.Text.RegularExpressions.Regex.Replace(inventory.StoneName.Trim().ToLower(), @"\s+", " ");
                int StoneIdFromDb = db.Stones.Where(x => x.StoneName.Trim().ToLower() == inventory.StoneName).Select(p => p.Id).DefaultIfEmpty(-1).Single();
                if (StoneIdFromDb > 0)
                    inventory.StoneId = StoneIdFromDb;
                else
                {
                    var stone = new Stone();
                    System.Globalization.TextInfo textInfo = new System.Globalization.CultureInfo("en-US", false).TextInfo;
                    stone.StoneName = textInfo.ToTitleCase(inventory.StoneName).Remove(inventory.StoneName.Length - 2) + inventory.StoneName.Substring(inventory.StoneName.Length - 2);
                    db.Stones.Add(stone);
                    db.SaveChanges();
                    inventory.StoneId = stone.Id;
                }

                int SlabCount = int.Parse(Request["txtSlabCount"]);
                int MaxSlabId = db.StoneInventorys.Where(x => x.StoneId == inventory.StoneId).Select(p=>p.Id).DefaultIfEmpty(1).Max();
                for (int i = 0; i < SlabCount; i++)
                {
                    var clone = new StoneInventory();                    
                    clone.StoneId = inventory.StoneId;
                    clone.SlabNo = (i + MaxSlabId).ToString("D2");
                    clone.LotNo = inventory.LotNo;
                    clone.VendorId = inventory.VendorId;
                    clone.Consignment = inventory.Consignment;
                    clone.DateAdded = inventory.DateAdded;
                    clone.Length = inventory.Length;
                    clone.Width = inventory.Width;
                    clone.UnitPrice = inventory.UnitPrice;
                    clone.SalesTax = inventory.SalesTax;
                    db.StoneInventorys.Add(clone);
                }

                db.SaveChanges();
                return Json(new
                {
                    success = true,
                    ctrl = "Inventory",
                    action = "insert",
                    id = inventory.Id.ToString()
                    //,col = new[] {
                    //    db.Stones.Where(x => x.Id == inventory.StoneId).Select(s => s.StoneName).FirstOrDefault(),
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

            ViewBag.StoneLookup = new SelectList(db.Stones.Where(x => x.Inactive == false)
                .Select(s => new SelectListItem { Value = s.Id.ToString(), Text = s.StoneName + " " + s.Thickness }), "Value", "Text");

            ViewBag.VendorLookup = new SelectList(db.Vendors.Where(x => x.Inactive == false)
                .Select(s => new SelectListItem { Value = s.Id.ToString(), Text = s.VendorName }), "Value", "Text");

            var inventory = db.StoneInventorys.Find(id);

            if (inventory == null)
            {
                return HttpNotFound();
            }

            return PartialView("Edit", inventory);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(StoneInventory inventory)
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
                        "<div data-role=\"display\"><span class=\"glyphicon glyphicon-plus\" style=\"cursor: pointer; \"></span></div>",
                        db.Stones.Where(x => x.Id == inventory.StoneId).Select(s => s.StoneName).FirstOrDefault(),
                        inventory.SerialNo,
                        inventory.LotNo,
                        db.Vendors.Where(x => x.Id == inventory.VendorId).Select(s => s.VendorName).FirstOrDefault(),
                        inventory.Consignment.ToString(),
                        String.Format("{0:MM-dd-yyyy}",inventory.DateAdded),
                        inventory.Length.ToString() + " x " + inventory.Width.ToString() + " = " + (Math.Round((double) (inventory.Length * inventory.Width) / 144.00, 2)).ToString(),
                        inventory.QuoteId == null ? null : inventory.Id + " - " + db.Quotes.Where(x => x.Id == inventory.QuoteId).Select(s => s.FirstName + " " + s.LastName).FirstOrDefault(),
                        inventory.UnitPrice.ToString(),
                        inventory.SalesTax.ToString(),
                        //((inventory.UnitPrice + inventory.UnitPrice * (inventory.SalesTax ?? 0)) * ((double)(inventory.Length * inventory.Width) / 144.00)).ToString(),
                        ((inventory.UnitPrice + inventory.UnitPrice * ((inventory.SalesTax/100) ?? 0)) * ((double)(inventory.Length * inventory.Width) / 144.00)).ToString(),
                        String.Format("{0:MM-dd-yyyy}",inventory.DateSold),
                    }
                });
            }
            return PartialView("Edit", inventory);
        }

        public string EditSerialNum(int id, string SerialNo)
        {
            try
            {
                db.StoneInventorys.Find(id).SerialNo = SerialNo;
                db.SaveChanges();
            }
            catch
            {
                return "false";
            }

            return "true";
        }

        public string EditSlabNum(int id, string SlabNo)
        {
            try
            {
                db.StoneInventorys.Find(id).SlabNo = SlabNo;
                db.SaveChanges();
            }
            catch
            {
                return "false";
            }

            return "true";
        }

        public ActionResult Delete(int id = 0)
        {
            StoneInventory inventory = db.StoneInventorys.Find(id);
            if (inventory == null)
            {
                return HttpNotFound();
            }

            inventory.StoneName = db.Stones.Where(x => x.Id == inventory.StoneId).Select(s => s.StoneName + " " + s.Thickness).FirstOrDefault();
            return PartialView("Delete", inventory);
        }


        //
        // POST: /Stone/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var inventory = db.StoneInventorys.Find(id);
            db.StoneInventorys.Remove(inventory);
            db.SaveChanges();
            return Json(new { success = true, action = "delete" });
        }

    }


        [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public sealed class NoCacheAttribute : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            filterContext.HttpContext.Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
            filterContext.HttpContext.Response.Cache.SetValidUntilExpires(false);
            filterContext.HttpContext.Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
            filterContext.HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            filterContext.HttpContext.Response.Cache.SetNoStore();

            base.OnResultExecuting(filterContext);
        }
    }

}