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
    public class AssignedtoController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: /Assignedto/
        public ActionResult Index(string filter = null, int page = 1, int pageSize = 5, string sort = "Id", string sortdir = "DESC", bool showInactive = false)
        {

            var records = new PagedList<Assignedto>();
            ViewBag.filter = filter;
            //if (!string.IsNullOrEmpty(filter))
            //{
            //    pageSize = 50;
            //}

            if (showInactive)
            {
                records.Content = db.Assignedtos
                            .Where(x => filter == null || x.AssignedtoName.Contains(filter))
                            .OrderBy(sort + " " + sortdir)
                            .Skip((page - 1) * pageSize)
                            .Take(pageSize)
                            .ToList();
                ViewBag.showInactive = false;
                ViewBag.labelInactive = "Hide";
            }
            else
            {
                records.Content = db.Assignedtos
                            .Where(x => (x.Inactive == false) && filter == null || x.AssignedtoName.Contains(filter))
                            .OrderBy(sort + " " + sortdir)
                            .Skip((page - 1) * pageSize)
                            .Take(pageSize)
                            .ToList();
                ViewBag.showInactive = true;
                ViewBag.labelInactive = "Show";
            }

            // Count
            records.TotalRecords = db.Assignedtos
                         .Where(x => filter == null || x.AssignedtoName.Contains(filter)).Count();

            records.CurrentPage = page;
            records.PageSize = pageSize;

            if (Request.IsAjaxRequest() || Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return PartialView("_Grid", records);
            else
                return View(records);

        }

        // GET: /Assignedto/Details
        public ActionResult Details(int id = 0)
        {
            Assignedto assignedto = db.Assignedtos.Find(id);
            if (assignedto == null)
            {
                return HttpNotFound();
            }
            return PartialView("Details", assignedto);
        }


        // GET: /Assignedto/Create
        [HttpGet]
        public ActionResult Create()
        {
            var assignedto = new Assignedto();
            return PartialView("Create", assignedto);
        }

        // POST: /Assignedto/Create
        [HttpPost]
        public JsonResult Create(Assignedto assignedto)
        {
            if (ModelState.IsValid)
            {
                db.Assignedtos.Add(assignedto);
                db.SaveChanges();
                return Json(new
                {
                    success = true,
                    ctrl = "Assignedto",
                    action = "insert",
                    id = assignedto.Id.ToString(),
                    col = new[] {
                        assignedto.AssignedtoName,
                        assignedto.Notes
                    }
                });
            }
            return Json(assignedto, JsonRequestBehavior.AllowGet);
        }

        // GET: /Assignedto/Edit/5
        [HttpGet]
        public ActionResult Edit(int id = 0)
        {
            var assignedto = db.Assignedtos.Find(id);
            if (assignedto == null)
            {
                return HttpNotFound();
            }

            return PartialView("Edit", assignedto);
        }

        // POST: /Assignedto/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Assignedto assignedto)
        {
            if (ModelState.IsValid)
            {
                db.Entry(assignedto).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new
                {
                    success = true,
                    action = "edit",
                    inactive = assignedto.Inactive.ToString(),
                    col = new[] {
                        assignedto.AssignedtoName,
                        assignedto.Notes
                    }
                });
            }
            return PartialView("Edit", assignedto);
        }

        //
        // GET: /Assignedto/Delete/5
        public ActionResult Delete(int id = 0)
        {
            Assignedto assignedto = db.Assignedtos.Find(id);
            if (assignedto == null)
            {
                return HttpNotFound();
            }
            return PartialView("Delete", assignedto);
        }


        //
        // POST: /Assignedto/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var assignedto = db.Assignedtos.Find(id);
            db.Assignedtos.Remove(assignedto);
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
