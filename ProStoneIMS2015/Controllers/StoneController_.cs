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
    [Authorize(Roles = "subscriber")]
    public class StoneController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Stone
        // GET: /Stone/
        public ActionResult Index(string filter = null, int page = 1, int pageSize = 5, string sort = "Id", string sortdir = "DESC", bool showInactive = false)
        {

            var records = new PagedList<Stone>();
            ViewBag.filter = filter;

            var qry = from s in db.Stones
                      join t in db.Stone_types
                      on s.Type equals t.Id
                      select new
                      {
                          Id = s.Id,
                          StoneName = s.StoneName,
                          Thickness = s.Thickness,
                          CatalogID = s.CatalogID,
                          TypeName = t.Name,
                          Price = s.Price,
                          Length = s.Length,
                          Width = s.Width,
                          LotNumber = s.LotNumber,
                          Inventory = s.Inventory,
                          WOPrice = s.WOPrice,
                          ImageFilename = s.ImageFilename,
                          OnPromo = s.OnPromo,
                          Inactive = s.Inactive
                      };

            if (showInactive)
            {
                records.Content = qry
                            .Where(x => filter == null || x.StoneName.Contains(filter))
                            .OrderBy(sort + " " + sortdir)
                            .Skip((page - 1) * pageSize)
                            .Take(pageSize)
                            .ToList()
                                .Select(s => new Stone()
                                {
                                    Id = s.Id,
                                    StoneName = s.StoneName + " " + s.Thickness,
                                    CatalogID = s.CatalogID,
                                    TypeName = s.TypeName,
                                    Price = s.Price,
                                    Length = s.Length,
                                    Width = s.Width,
                                    LotNumber = s.LotNumber,
                                    Inventory = s.Inventory,
                                    WOPrice = s.WOPrice,
                                    ImageFilename = s.ImageFilename,
                                    OnPromo = s.OnPromo,
                                    Inactive = s.Inactive
                                }).ToList();
                ViewBag.showInactive = false;
                ViewBag.labelInactive = "Hide";
            }
            else
            {
                records.Content = qry
                            .Where(x => (x.Inactive == false) && filter == null || x.StoneName.Contains(filter))
                            .OrderBy(sort + " " + sortdir)
                            .Skip((page - 1) * pageSize)
                            .Take(pageSize)
                            .ToList()
                                .Select(s => new Stone()
                                {
                                    Id = s.Id,
                                    StoneName = s.StoneName + " " + s.Thickness,
                                    CatalogID = s.CatalogID,
                                    TypeName = s.TypeName,
                                    Price = s.Price,
                                    Length = s.Length,
                                    Width = s.Width,
                                    LotNumber = s.LotNumber,
                                    Inventory = s.Inventory,
                                    WOPrice = s.WOPrice,
                                    ImageFilename = s.ImageFilename,
                                    OnPromo = s.OnPromo,
                                    Inactive = s.Inactive
                                }).ToList();
                ViewBag.showInactive = true;
                ViewBag.labelInactive = "Show";
            }

            // Count
            records.TotalRecords = db.Stones
                         .Where(x => filter == null || x.StoneName.Contains(filter)).Count();

            records.CurrentPage = page;
            records.PageSize = pageSize;

            if (Request.IsAjaxRequest() || Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return PartialView("_Grid", records);
            else
                return View(records);

        }

        public ActionResult Details(int id = 0)
        {
            Stone stone = db.Stones.Find(id);
            if (stone == null)
            {
                return HttpNotFound();
            }
            stone.StoneNameCm = stone.StoneName + " " + stone.Thickness;
            stone.TypeName = db.Stone_types.Where(x => x.Id == stone.Type).Select(s => s.Name).FirstOrDefault().ToString();
            return PartialView("Details", stone);
        }

        private IEnumerable<SelectListItem> GetStoneType()
        {
            var record = db.Stone_types
                .Where(x => x.Inactive == false)
                .Select(s => new SelectListItem { Value = s.Id.ToString(), Text = s.Name });

            return new SelectList(record, "Value", "Text");
        }

        // GET: /Stone/Create
        [HttpGet]
        public ActionResult Create()
        {
            var stone = new Stone();
            ViewData["Stone_type"] = GetStoneType();
            return PartialView("Create", stone);
        }

        // POST: /Stone/Create
        [HttpPost]
        public JsonResult Create(Stone stone)
        {
            if (ModelState.IsValid)
            {
                db.Stones.Add(stone);
                db.SaveChanges();
                return Json(new
                {
                    success = true,
                    ctrl = "Stone",
                    action = "insert",
                    id = stone.Id.ToString(),
                    col = new[] {
                        stone.StoneName + " " + stone.Thickness,
                        stone.CatalogID,
                        db.Stone_types.Where(x => x.Id == stone.Type).Select(s => s.Name).FirstOrDefault().ToString(),
                        string.Format("{0:C}", stone.Price),
                        string.Format("{0:C}", stone.WOPrice),
                        stone.Length.ToString(),
                        stone.Width.ToString(),
                        stone.Inventory.ToString(),
                        stone.LotNumber,
                        stone.OnPromo.ToString(),
                        stone.ImageFilename,
                    }
                });
            }
            return Json(stone, JsonRequestBehavior.AllowGet);
        }

        // GET: /Stone/Edit/5
        [HttpGet]
        public ActionResult Edit(int id = 0)
        {
            var stone = db.Stones.Find(id);
            ViewData["Stone_type"] = GetStoneType();

            if (stone == null)
            {
                return HttpNotFound();
            }

            return PartialView("Edit", stone);
        }

        // POST: /Stone/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Stone stone)
        {
            if (ModelState.IsValid)
            {
                db.Entry(stone).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new
                {
                    success = true,
                    action = "edit",
                    inactive = stone.Inactive.ToString(),
                    col = new[] {
                        stone.StoneName + " " + stone.Thickness,
                        stone.CatalogID,
                        db.Stone_types.Where(x => x.Id == stone.Type).Select(s => s.Name).FirstOrDefault().ToString(),
                        string.Format("{0:C}", stone.Price),
                        string.Format("{0:C}", stone.WOPrice),
                        stone.Length.ToString(),
                        stone.Width.ToString(),
                        stone.Inventory.ToString(),
                        stone.LotNumber,
                        stone.OnPromo.ToString(),
                        stone.ImageFilename,
                    }
                });
            }
            return PartialView("Edit", stone);
        }

        //
        // GET: /Stone/Delete/5
        public ActionResult Delete(int id = 0)
        {
            Stone stone = db.Stones.Find(id);
            if (stone == null)
            {
                return HttpNotFound();
            }
            stone.StoneNameCm = stone.StoneName + " " + stone.Thickness;
            return PartialView("Delete", stone);
        }


        //
        // POST: /Stone/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var stone = db.Stones.Find(id);
            db.Stones.Remove(stone);
            db.SaveChanges();
            return Json(new { success = true, action = "delete" });
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

    }
}