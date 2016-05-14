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
    [Authorize(Roles = "admin")]
    public class Jservice_typeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Jservice_type
        // GET: /Jservice_type/
        public ActionResult Index(string filter = null, int page = 1, int pageSize = 5, string sort = "Id", string sortdir = "DESC", bool showInactive = false)
        {

            var records = new PagedList<Jservice_type>();
            ViewBag.filter = filter;
            //if (!string.IsNullOrEmpty(filter))
            //{
            //    pageSize = 50;
            //}

            if (showInactive)
            {
                records.Content = db.Jservice_types
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
                records.Content = db.Jservice_types
                            .Where(x => (x.Inactive == false) && filter == null || x.Name.Contains(filter))
                            .OrderBy(sort + " " + sortdir)
                            .Skip((page - 1) * pageSize)
                            .Take(pageSize)
                            .ToList();
                ViewBag.showInactive = true;
                ViewBag.labelInactive = "Show";
            }

            // Count
            records.TotalRecords = db.Jservice_types
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
            Jservice_type Jservice_type = db.Jservice_types.Find(id);
            if (Jservice_type == null)
            {
                return HttpNotFound();
            }
            return PartialView("Details", Jservice_type);
        }

        // GET: /Jservice_type/Create
        [HttpGet]
        public ActionResult Create()
        {
            var Jservice_type = new Jservice_type();
            return PartialView("Create", Jservice_type);
        }

        // POST: /Jservice_type/Create
        [HttpPost]
        public JsonResult Create(Jservice_type Jservice_type)
        {
            if (ModelState.IsValid)
            {
                db.Jservice_types.Add(Jservice_type);
                db.SaveChanges();
                return Json(new
                {
                    success = true,
                    ctrl = "Jservice_type",
                    action = "insert",
                    id = Jservice_type.Id.ToString(),
                    col = new[] {
                        Jservice_type.Name,
                        Jservice_type.Notes
                    }
                });
            }
            return Json(Jservice_type, JsonRequestBehavior.AllowGet);
        }

        // GET: /Jservice_type/Edit/5
        [HttpGet]
        public ActionResult Edit(int id = 0)
        {
            var Jservice_type = db.Jservice_types.Find(id);
            if (Jservice_type == null)
            {
                return HttpNotFound();
            }

            return PartialView("Edit", Jservice_type);
        }

        // POST: /Jservice_type/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Jservice_type Jservice_type)
        {
            if (ModelState.IsValid)
            {
                db.Entry(Jservice_type).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new
                {
                    success = true,
                    action = "edit",
                    inactive = Jservice_type.Inactive.ToString(),
                    col = new[] {
                        Jservice_type.Name,
                        Jservice_type.Notes
                    }
                });
            }
            return PartialView("Edit", Jservice_type);
        }

        //
        // GET: /Jservice_type/Delete/5
        public ActionResult Delete(int id = 0)
        {
            Jservice_type Jservice_type = db.Jservice_types.Find(id);
            if (Jservice_type == null)
            {
                return HttpNotFound();
            }
            return PartialView("Delete", Jservice_type);
        }


        //
        // POST: /Jservice_type/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var Jservice_type = db.Jservice_types.Find(id);
            db.Jservice_types.Remove(Jservice_type);
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