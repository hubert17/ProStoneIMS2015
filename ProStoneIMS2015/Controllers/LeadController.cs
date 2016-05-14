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
    public class LeadController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Lead
        // GET: /Lead/
        public ActionResult Index(string filter = null, int page = 1, int pageSize = 5, string sort = "Id", string sortdir = "DESC", bool showInactive = false)
        {

            var records = new PagedList<Lead>();
            ViewBag.filter = filter;
            //if (!string.IsNullOrEmpty(filter))
            //{
            //    pageSize = 50;
            //}

            if (showInactive)
            {
                records.Content = db.Leads
                            .Where(x => filter == null || x.LeadName.Contains(filter))
                            .OrderBy(sort + " " + sortdir)
                            .Skip((page - 1) * pageSize)
                            .Take(pageSize)
                            .ToList();
                ViewBag.showInactive = false;
                ViewBag.labelInactive = "Hide";
            }
            else
            {
                records.Content = db.Leads
                            .Where(x => (x.Inactive == false) && filter == null || x.LeadName.Contains(filter))
                            .OrderBy(sort + " " + sortdir)
                            .Skip((page - 1) * pageSize)
                            .Take(pageSize)
                            .ToList();
                ViewBag.showInactive = true;
                ViewBag.labelInactive = "Show";
            }

            // Count
            records.TotalRecords = db.Leads
                         .Where(x => filter == null || x.LeadName.Contains(filter)).Count();

            records.CurrentPage = page;
            records.PageSize = pageSize;

            if (Request.IsAjaxRequest() || Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return PartialView("_Grid", records);
            else
                return View(records);

        }

        public ActionResult Details(int id = 0)
        {
            Lead lead = db.Leads.Find(id);
            if (lead == null)
            {
                return HttpNotFound();
            }
            return PartialView("Details", lead);
        }


        // GET: /Lead/Create
        [HttpGet]
        public ActionResult Create()
        {
            var lead = new Lead();
            return PartialView("Create", lead);
        }

        // POST: /Lead/Create
        [HttpPost]
        public JsonResult Create(Lead lead)
        {
            if (ModelState.IsValid)
            {
                db.Leads.Add(lead);
                db.SaveChanges();
                return Json(new
                {
                    success = true,
                    ctrl = "Lead",
                    action = "insert",
                    id = lead.Id.ToString(),
                    col = new[] {
                        lead.LeadName,
                        lead.Notes
                    }
                });
            }
            return Json(lead, JsonRequestBehavior.AllowGet);
        }

        // GET: /Lead/Edit/5
        [HttpGet]
        public ActionResult Edit(int id = 0)
        {
            var lead = db.Leads.Find(id);
            if (lead == null)
            {
                return HttpNotFound();
            }

            return PartialView("Edit", lead);
        }

        // POST: /Lead/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Lead lead)
        {
            if (ModelState.IsValid)
            {
                db.Entry(lead).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new
                {
                    success = true,
                    action = "edit",
                    inactive = lead.Inactive.ToString(),
                    col = new[] {
                        lead.LeadName,
                        lead.Notes
                    }
                });
            }
            return PartialView("Edit", lead);
        }

        //
        // GET: /Lead/Delete/5
        public ActionResult Delete(int id = 0)
        {
            Lead lead = db.Leads.Find(id);
            if (lead == null)
            {
                return HttpNotFound();
            }
            return PartialView("Delete", lead);
        }


        //
        // POST: /Lead/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var lead = db.Leads.Find(id);
            db.Leads.Remove(lead);
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