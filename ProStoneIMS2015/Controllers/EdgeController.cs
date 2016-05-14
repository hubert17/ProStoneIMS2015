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
    public class EdgeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Edge
        // GET: /Edge/
        public ActionResult Index(string filter = null, int page = 1, int pageSize = 5, string sort = "Id", string sortdir = "DESC", bool showInactive = false)
        {

            var records = new PagedList<Edge>();
            ViewBag.filter = filter;
            //if (!string.IsNullOrEmpty(filter))
            //{
            //    pageSize = 50;
            //}

            if (showInactive)
            {
                records.Content = db.Edges
                            .Where(x => filter == null || x.EdgeName.Contains(filter))
                            .OrderBy(sort + " " + sortdir)
                            .Skip((page - 1) * pageSize)
                            .Take(pageSize)
                            .ToList();
                ViewBag.showInactive = false;
                ViewBag.labelInactive = "Hide";
            }
            else
            {
                records.Content = db.Edges
                            .Where(x => (x.Inactive == false) && filter == null || x.EdgeName.Contains(filter))
                            .OrderBy(sort + " " + sortdir)
                            .Skip((page - 1) * pageSize)
                            .Take(pageSize)
                            .ToList();
                ViewBag.showInactive = true;
                ViewBag.labelInactive = "Show";
            }

            // Count
            records.TotalRecords = db.Edges
                         .Where(x => filter == null || x.EdgeName.Contains(filter)).Count();

            records.CurrentPage = page;
            records.PageSize = pageSize;

            if (Request.IsAjaxRequest() || Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return PartialView("_Grid", records);
            else
                return View(records);

        }

        public ActionResult Details(int id = 0)
        {
            Edge edge = db.Edges.Find(id);
            if (edge == null)
            {
                return HttpNotFound();
            }
            return PartialView("Details", edge);
        }


        // GET: /Edge/Create
        [HttpGet]
        public ActionResult Create()
        {
            var edge = new Edge();
            return PartialView("Create", edge);
        }

        // POST: /Edge/Create
        [HttpPost]
        public JsonResult Create(Edge edge)
        {
            if (ModelState.IsValid)
            {
                db.Edges.Add(edge);
                db.SaveChanges();
                return Json(new
                {
                    success = true,
                    ctrl = "Edge",
                    action = "insert",
                    id = edge.Id.ToString(),
                    col = new[] {
                        edge.EdgeName,
                        string.Format("{0:C}", edge.Price),
                        edge.ImageFilename
                    }
                });
            }
            return Json(edge, JsonRequestBehavior.AllowGet);
        }

        // GET: /Edge/Edit/5
        [HttpGet]
        public ActionResult Edit(int id = 0)
        {
            var edge = db.Edges.Find(id);
            if (edge == null)
            {
                return HttpNotFound();
            }

            return PartialView("Edit", edge);
        }

        // POST: /Edge/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Edge edge)
        {
            if (ModelState.IsValid)
            {
                db.Entry(edge).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new
                {
                    success = true,
                    action = "edit",
                    inactive = edge.Inactive.ToString(),
                    col = new[] {
                        edge.EdgeName,
                        string.Format("{0:C}", edge.Price),
                        edge.ImageFilename
                    }
                });
            }
            return PartialView("Edit", edge);
        }

        //
        // GET: /Edge/Delete/5
        public ActionResult Delete(int id = 0)
        {
            Edge edge = db.Edges.Find(id);
            if (edge == null)
            {
                return HttpNotFound();
            }
            return PartialView("Delete", edge);
        }


        //
        // POST: /Edge/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var edge = db.Edges.Find(id);
            db.Edges.Remove(edge);
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