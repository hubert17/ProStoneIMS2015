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
    public class VendorController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Vendor
        // GET: /Vendor/
        public ActionResult Index(string filter = null, int page = 1, int pageSize = 5, string sort = "Id", string sortdir = "DESC", bool showInactive = false)
        {

            var records = new PagedList<Vendor>();
            ViewBag.filter = filter;
            //if (!string.IsNullOrEmpty(filter))
            //{
            //    pageSize = 50;
            //}

            if (showInactive)
            {
                records.Content = db.Vendors
                            .Where(x => filter == null || x.VendorName.Contains(filter))
                            .OrderBy(sort + " " + sortdir)
                            .Skip((page - 1) * pageSize)
                            .Take(pageSize)
                            .ToList();
                ViewBag.showInactive = false;
                ViewBag.labelInactive = "Hide";
            }
            else
            {
                records.Content = db.Vendors
                            .Where(x => (x.Inactive == false) && filter == null || x.VendorName.Contains(filter))
                            .OrderBy(sort + " " + sortdir)
                            .Skip((page - 1) * pageSize)
                            .Take(pageSize)
                            .ToList();
                ViewBag.showInactive = true;
                ViewBag.labelInactive = "Show";
            }

            // Count
            records.TotalRecords = db.Vendors
                         .Where(x => filter == null || x.VendorName.Contains(filter)).Count();

            records.CurrentPage = page;
            records.PageSize = pageSize;

            if (Request.IsAjaxRequest() || Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return PartialView("_Grid", records);
            else
                return View(records);

        }

        public ActionResult Details(int id = 0)
        {
            Vendor vendor = db.Vendors.Find(id);
            if (vendor == null)
            {
                return HttpNotFound();
            }
            return PartialView("Details", vendor);
        }


        // GET: /Vendor/Create
        [HttpGet]
        public ActionResult Create()
        {
            var vendor = new Vendor();
            return PartialView("Create", vendor);
        }

        // POST: /Vendor/Create
        [HttpPost]
        public JsonResult Create(Vendor vendor)
        {
            if (ModelState.IsValid)
            {
                db.Vendors.Add(vendor);
                db.SaveChanges();
                return Json(new
                {
                    success = true,
                    ctrl = "Vendor",
                    action = "insert",
                    id = vendor.Id.ToString(),
                    col = new[] {
                        vendor.VendorName,
                        vendor.Notes
                    }
                });
            }
            return Json(vendor, JsonRequestBehavior.AllowGet);
        }

        // GET: /Vendor/Edit/5
        [HttpGet]
        public ActionResult Edit(int id = 0)
        {
            var vendor = db.Vendors.Find(id);
            if (vendor == null)
            {
                return HttpNotFound();
            }

            return PartialView("Edit", vendor);
        }

        // POST: /Vendor/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Vendor vendor)
        {
            if (ModelState.IsValid)
            {
                db.Entry(vendor).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new
                {
                    success = true,
                    action = "edit",
                    inactive = vendor.Inactive.ToString(),
                    col = new[] {
                        vendor.VendorName,
                        vendor.Notes
                    }
                });
            }
            return PartialView("Edit", vendor);
        }

        //
        // GET: /Vendor/Delete/5
        public ActionResult Delete(int id = 0)
        {
            Vendor vendor = db.Vendors.Find(id);
            if (vendor == null)
            {
                return HttpNotFound();
            }
            return PartialView("Delete", vendor);
        }


        //
        // POST: /Vendor/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var vendor = db.Vendors.Find(id);
            db.Vendors.Remove(vendor);
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