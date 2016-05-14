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
    public class SalesmanController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: /Salesman/
        public ActionResult Index(string filter = null, int page = 1, int pageSize = 5, string sort = "Id", string sortdir = "DESC", bool showInactive = false)
        {

            var records = new PagedList<Salesman>();
            ViewBag.filter = filter;
            //if (!string.IsNullOrEmpty(filter))
            //{
            //    pageSize = 50;
            //}

            if (showInactive)
            {
                records.Content = db.Salesmans
                            .Where(x => filter == null || x.SalesmanName.Contains(filter))
                            .OrderBy(sort + " " + sortdir)
                            .Skip((page - 1) * pageSize)
                            .Take(pageSize)
                            .ToList();
                ViewBag.showInactive = false;
                ViewBag.labelInactive = "Hide";
            }
            else
            {
                records.Content = db.Salesmans
                            .Where(x => (x.Inactive == false) && filter == null || x.SalesmanName.Contains(filter))
                            .OrderBy(sort + " " + sortdir)
                            .Skip((page - 1) * pageSize)
                            .Take(pageSize)
                            .ToList();
                ViewBag.showInactive = true;
                ViewBag.labelInactive = "Show";
            }

            // Count
            records.TotalRecords = db.Salesmans
                         .Where(x => filter == null || x.SalesmanName.Contains(filter)).Count();

            records.CurrentPage = page;
            records.PageSize = pageSize;

            if (Request.IsAjaxRequest() || Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return PartialView("_Grid", records);
            else
                return View(records);

        }

        // GET: /Salesman/Details
        public ActionResult Details(int id = 0)
        {
            Salesman salesman = db.Salesmans.Find(id);
            if (salesman == null)
            {
                return HttpNotFound();
            }
            return PartialView("Details", salesman);
        }


        // GET: /Salesman/Create
        [HttpGet]
        public ActionResult Create()
        {
            var salesman = new Salesman();
            return PartialView("Create", salesman);
        }

        // POST: /Salesman/Create
        [HttpPost]
        public JsonResult Create(Salesman salesman)
        {
            if (ModelState.IsValid)
            {
                db.Salesmans.Add(salesman);
                db.SaveChanges();
                return Json(new
                {
                    success = true,
                    ctrl = "Salesman",
                    action = "insert",
                    id = salesman.Id.ToString(),
                    col = new[] {
                        salesman.SalesmanName,
                        salesman.Notes
                    }
                });
            }
            return Json(salesman, JsonRequestBehavior.AllowGet);
        }

        // GET: /Salesman/Edit/5
        [HttpGet]
        public ActionResult Edit(int id = 0)
        {
            var salesman = db.Salesmans.Find(id);
            if (salesman == null)
            {
                return HttpNotFound();
            }

            return PartialView("Edit", salesman);
        }

        // POST: /Salesman/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Salesman salesman)
        {
            if (ModelState.IsValid)
            {
                db.Entry(salesman).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new
                {
                    success = true,
                    action = "edit",
                    inactive = salesman.Inactive.ToString(),
                    col = new[] {
                        salesman.SalesmanName,
                        salesman.Notes
                    }
                });
            }
            return PartialView("Edit", salesman);
        }

        //
        // GET: /Salesman/Delete/5
        public ActionResult Delete(int id = 0)
        {
            Salesman salesman = db.Salesmans.Find(id);
            if (salesman == null)
            {
                return HttpNotFound();
            }
            return PartialView("Delete", salesman);
        }


        //
        // POST: /Salesman/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var salesman = db.Salesmans.Find(id);
            db.Salesmans.Remove(salesman);
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
