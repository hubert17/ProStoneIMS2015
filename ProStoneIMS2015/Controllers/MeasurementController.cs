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
    public class MeasurementController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Measurement
        // GET: /Measurement/
        public ActionResult Index(string filter = null, int page = 1, int pageSize = 5, string sort = "Id", string sortdir = "DESC", bool showInactive = false)
        {

            var records = new PagedList<Measurement>();
            ViewBag.filter = filter;
            //if (!string.IsNullOrEmpty(filter))
            //{
            //    pageSize = 50;
            //}

            if (showInactive)
            {
                records.Content = db.Measurements
                            .Where(x => filter == null || x.MeasureName.Contains(filter))
                            .OrderBy(sort + " " + sortdir)
                            .Skip((page - 1) * pageSize)
                            .Take(pageSize)
                            .ToList();
                ViewBag.showInactive = false;
                ViewBag.labelInactive = "Hide";
            }
            else
            {
                records.Content = db.Measurements
                            .Where(x => (x.Inactive == false) && filter == null || x.MeasureName.Contains(filter))
                            .OrderBy(sort + " " + sortdir)
                            .Skip((page - 1) * pageSize)
                            .Take(pageSize)
                            .ToList();
                ViewBag.showInactive = true;
                ViewBag.labelInactive = "Show";
            }

            // Count
            records.TotalRecords = db.Measurements
                         .Where(x => filter == null || x.MeasureName.Contains(filter)).Count();

            records.CurrentPage = page;
            records.PageSize = pageSize;

            if (Request.IsAjaxRequest() || Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return PartialView("_Grid", records);
            else
                return View(records);

        }

        public ActionResult Details(int id = 0)
        {
            Measurement measurement = db.Measurements.Find(id);
            if (measurement == null)
            {
                return HttpNotFound();
            }
            return PartialView("Details", measurement);
        }


        // GET: /Measurement/Create
        [HttpGet]
        public ActionResult Create()
        {
            var measurement = new Measurement();
            return PartialView("Create", measurement);
        }

        // POST: /Measurement/Create
        [HttpPost]
        public JsonResult Create(Measurement measurement)
        {
            if (ModelState.IsValid)
            {
                db.Measurements.Add(measurement);
                db.SaveChanges();
                return Json(new
                {
                    success = true,
                    ctrl = "Measurement",
                    action = "insert",
                    id = measurement.Id.ToString(),
                    col = new[] {
                        measurement.MeasureName,
                        measurement.Notes
                    }
                });
            }
            return Json(measurement, JsonRequestBehavior.AllowGet);
        }

        // GET: /Measurement/Edit/5
        [HttpGet]
        public ActionResult Edit(int id = 0)
        {
            var measurement = db.Measurements.Find(id);
            if (measurement == null)
            {
                return HttpNotFound();
            }

            return PartialView("Edit", measurement);
        }

        // POST: /Measurement/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Measurement measurement)
        {
            if (ModelState.IsValid)
            {
                db.Entry(measurement).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new
                {
                    success = true,
                    action = "edit",
                    inactive = measurement.Inactive.ToString(),
                    col = new[] {
                        measurement.MeasureName,
                        measurement.Notes
                    }
                });
            }
            return PartialView("Edit", measurement);
        }

        //
        // GET: /Measurement/Delete/5
        public ActionResult Delete(int id = 0)
        {
            Measurement measurement = db.Measurements.Find(id);
            if (measurement == null)
            {
                return HttpNotFound();
            }
            return PartialView("Delete", measurement);
        }


        //
        // POST: /Measurement/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var measurement = db.Measurements.Find(id);
            db.Measurements.Remove(measurement);
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