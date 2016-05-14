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
    public class JserviceController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Jservice
        // GET: /Jservice/
        public ActionResult Index(string filter = null, int page = 1, int pageSize = 5, string sort = "Id", string sortdir = "DESC", bool showInactive = false)
        {

            var records = new PagedList<Jservice>();
            ViewBag.filter = filter;
            //if (!string.IsNullOrEmpty(filter))
            //{
            //    pageSize = 50;
            //}

            var qry = from s in db.Jservices
                      join t in db.Jservice_types
                      on s.ServiceCode equals t.Code into st
                      from t in st.DefaultIfEmpty()
                      select new
                      {
                          Id = s.Id,
                          ServiceName = s.ServiceName,
                          Price = s.Price,
                          WOPrice = s.WOPrice,
                          ServiceCodeName = t.Name,
                          EnableCustomerView = s.EnableCustomerView,
                          Inactive = s.Inactive
                      };

            if (showInactive)
            {
                records.Content = qry
                            .Where(x => filter == null || x.ServiceName.Contains(filter))
                            .OrderBy(sort + " " + sortdir)
                            .Skip((page - 1) * pageSize)
                            .Take(pageSize)
                            .ToList()
                                .Select(s => new Jservice()
                                {
                                    Id = s.Id,
                                    ServiceName = s.ServiceName,
                                    Price = s.Price,
                                    WOPrice = s.WOPrice,
                                    ServiceCodeName = s.ServiceCodeName,
                                    EnableCustomerView = s.EnableCustomerView,
                                    Inactive = s.Inactive
                                }).ToList();
                ViewBag.showInactive = false;
                ViewBag.labelInactive = "Hide";
            }
            else
            {
                records.Content = qry
                            .Where(x => (x.Inactive == false) && filter == null || x.ServiceName.Contains(filter))
                            .OrderBy(sort + " " + sortdir)
                            .Skip((page - 1) * pageSize)
                            .Take(pageSize)
                            .ToList()
                                .Select(s => new Jservice()
                                {
                                    Id = s.Id,
                                    ServiceName = s.ServiceName,
                                    Price = s.Price,
                                    WOPrice = s.WOPrice,
                                    ServiceCodeName = s.ServiceCodeName,
                                    EnableCustomerView = s.EnableCustomerView,
                                    Inactive = s.Inactive
                                }).ToList();
                ViewBag.showInactive = true;
                ViewBag.labelInactive = "Show";
            }

            // Count
            records.TotalRecords = db.Jservices
                         .Where(x => filter == null || x.ServiceName.Contains(filter)).Count();

            records.CurrentPage = page;
            records.PageSize = pageSize;

            if (Request.IsAjaxRequest() || Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return PartialView("_Grid", records);
            else
                return View(records);

        }

        public ActionResult Details(int id = 0)
        {
            Jservice jservice = db.Jservices.Find(id);
            if (jservice == null)
            {
                return HttpNotFound();
            }
            return PartialView("Details", jservice);
        }

        private IEnumerable<SelectListItem> GetServiceType()
        {
            var record = db.Jservice_types
                .Where(x => x.Inactive == false)
                .Select(s => new SelectListItem { Value = s.Code, Text = s.Name, Selected = (s.Code == String.Empty) });

            return new SelectList(record, "Value", "Text");
        }

        // GET: /Jservice/Create
        [HttpGet]
        public ActionResult Create()
        {
            var jservice = new Jservice();
            ViewData["Service_type"] = GetServiceType();
            return PartialView("Create", jservice);
        }

        // POST: /Jservice/Create
        [HttpPost]
        public JsonResult Create(Jservice jservice)
        {
            if (ModelState.IsValid)
            {
                db.Jservices.Add(jservice);
                db.SaveChanges();
                return Json(new
                {
                    success = true,
                    ctrl = "Jservice",
                    action = "insert",
                    id = jservice.Id.ToString(),
                    col = new[] {
                        jservice.ServiceName,
                        string.Format("{0:C}", jservice.Price),
                        string.Format("{0:C}", jservice.WOPrice),
                        db.Jservice_types.Where(x => x.Code == jservice.ServiceCode).Select(s => s.Name).FirstOrDefault().ToString(),
                        //jservice.EnableCustomerView.ToString()
                    }
                });
            }
            return Json(jservice, JsonRequestBehavior.AllowGet);
        }

        // GET: /Jservice/Edit/5
        [HttpGet]
        public ActionResult Edit(int id = 0)
        {
            var jservice = db.Jservices.Find(id);
            if (jservice == null)
            {
                return HttpNotFound();
            }
            ViewData["Service_type"] = GetServiceType();

            return PartialView("Edit", jservice);
        }

        // POST: /Jservice/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Jservice jservice)
        {
            if (ModelState.IsValid)
            {
                db.Entry(jservice).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new
                {
                    success = true,
                    action = "edit",
                    inactive = jservice.Inactive.ToString(),
                    col = new[] {
                        jservice.ServiceName,
                        string.Format("{0:C}", jservice.Price),
                        string.Format("{0:C}", jservice.WOPrice),
                        db.Jservice_types.Where(x => x.Code == jservice.ServiceCode).Select(s => s.Name).FirstOrDefault().ToString(),
                        //jservice.EnableCustomerView.ToString()
                    }
                });
            }
            return PartialView("Edit", jservice);
        }

        //
        // GET: /Jservice/Delete/5
        public ActionResult Delete(int id = 0)
        {
            Jservice jservice = db.Jservices.Find(id);
            if (jservice == null)
            {
                return HttpNotFound();
            }
            return PartialView("Delete", jservice);
        }


        //
        // POST: /Jservice/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var jservice = db.Jservices.Find(id);
            db.Jservices.Remove(jservice);
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