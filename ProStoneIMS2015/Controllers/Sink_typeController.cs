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
    public class Sink_typeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Sink_type
        // GET: /Sink_type/
        public ActionResult Index(string filter = null, int page = 1, int pageSize = 5, string sort = "Id", string sortdir = "DESC", bool showInactive = false)
        {

            var records = new PagedList<Sink_type>();
            ViewBag.filter = filter;
            //if (!string.IsNullOrEmpty(filter))
            //{
            //    pageSize = 50;
            //}

            if (showInactive)
            {
                records.Content = db.Sink_types
                            .Where(x => filter == null || x.Name.Contains(filter))
                            .OrderBy(sort + " " + sortdir)
                            .Skip((page - 1) * pageSize)
                            .Take(pageSize)
                            .ToList();
                ViewBag.showInactive = false;
                ViewBag.labelInactive = "Hide";
            }
            else
            {
                records.Content = db.Sink_types
                            .Where(x => (x.Inactive == false) && filter == null || x.Name.Contains(filter))
                            .OrderBy(sort + " " + sortdir)
                            .Skip((page - 1) * pageSize)
                            .Take(pageSize)
                            .ToList();
                ViewBag.showInactive = true;
                ViewBag.labelInactive = "Show";
            }

            // Count
            records.TotalRecords = db.Sink_types
                         .Where(x => filter == null || x.Name.Contains(filter)).Count();

            records.CurrentPage = page;
            records.PageSize = pageSize;

            if (Request.IsAjaxRequest() || Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return PartialView("_Grid", records);
            else
                return View(records);

        }

        public ActionResult Details(int id = 0)
        {
            Sink_type sink_type = db.Sink_types.Find(id);
            if (sink_type == null)
            {
                return HttpNotFound();
            }
            return PartialView("Details", sink_type);
        }

        // GET: /Sink_type/Create
        [HttpGet]
        public ActionResult Create()
        {
            var sink_type = new Sink_type();
            return PartialView("Create", sink_type);
        }

        // POST: /Sink_type/Create
        [HttpPost]
        public JsonResult Create(Sink_type sink_type)
        {
            if (ModelState.IsValid)
            {
                db.Sink_types.Add(sink_type);
                db.SaveChanges();
                return Json(new
                {
                    success = true,
                    ctrl = "Sink_type",
                    action = "insert",
                    id = sink_type.Id.ToString(),
                    col = new[] {
                        sink_type.Name,
                        sink_type.Notes
                    }
                });
            }
            return Json(sink_type, JsonRequestBehavior.AllowGet);
        }

        // GET: /Sink_type/Edit/5
        [HttpGet]
        public ActionResult Edit(int id = 0)
        {
            var sink_type = db.Sink_types.Find(id);
            if (sink_type == null)
            {
                return HttpNotFound();
            }

            return PartialView("Edit", sink_type);
        }

        // POST: /Sink_type/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Sink_type sink_type)
        {
            if (ModelState.IsValid)
            {
                db.Entry(sink_type).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new
                {
                    success = true,
                    action = "edit",
                    inactive = sink_type.Inactive.ToString(),
                    col = new[] {
                        sink_type.Name,
                        sink_type.Notes
                    }
                });
            }
            return PartialView("Edit", sink_type);
        }

        //
        // GET: /Sink_type/Delete/5
        public ActionResult Delete(int id = 0)
        {
            Sink_type sink_type = db.Sink_types.Find(id);
            if (sink_type == null)
            {
                return HttpNotFound();
            }
            return PartialView("Delete", sink_type);
        }


        //
        // POST: /Sink_type/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var sink_type = db.Sink_types.Find(id);
            db.Sink_types.Remove(sink_type);
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