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
    //[Authorize(Roles = "admin")]
    [Authorize]
    public class PhoneController : Controller
    {
        private ApplicationDbContext  db = new ApplicationDbContext();


        // GET: /Phone/
        public ActionResult Index(string filter = null, int page = 1, int pageSize = 5, string sort = "PhoneId", string sortdir = "DESC", bool showInactive = false)
        {

            var records = new PagedList<Phone>();
            ViewBag.filter = filter;
            //if (!string.IsNullOrEmpty(filter))
            //{
            //    pageSize = 50;
            //}

            if (showInactive)
            {
                records.Content = db.Phones
                            .Where( x => filter == null || (x.Model.Contains(filter)) || x.Company.Contains(filter) )
                            .OrderBy(sort + " " + sortdir)
                            .Skip((page - 1) * pageSize)
                            .Take(pageSize)
                            .ToList();
                ViewBag.showInactive = false;
                ViewBag.labelInactive = "Hide";
            }
            else
            {
                records.Content = db.Phones
                            .Where(x => (x.Inactive == false) && filter == null || (x.Model.Contains(filter)) || x.Company.Contains(filter))
                            .OrderBy(sort + " " + sortdir)
                            .Skip((page - 1) * pageSize)
                            .Take(pageSize)
                            .ToList();
                ViewBag.showInactive = true;
                ViewBag.labelInactive = "Show";
            }

            // Count
            records.TotalRecords = db.Phones
                         .Where(x => filter == null ||
                               (x.Model.Contains(filter)) || x.Company.Contains(filter)).Count();

            records.CurrentPage = page;
            records.PageSize = pageSize;

            if (Request.IsAjaxRequest() || Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return PartialView("_Grid", records);
            else
                return View(records);

        }

        public ActionResult IndexAjax(string filter = null, int page = 1, int pageSize = 5, string sort = "PhoneId", string sortdir = "DESC", bool showInactive = false)
        {

            var records = new PagedList<Phone>();
            ViewBag.filter = filter;
            if (showInactive)
            {
                records.Content = db.Phones
                            .Where(x => filter == null || (x.Model.Contains(filter)) || x.Company.Contains(filter))
                            .OrderBy(sort + " " + sortdir)
                            .Skip((page - 1) * pageSize)
                            .Take(pageSize)
                            .ToList();
                ViewBag.showInactive = false;
                ViewBag.labelInactive = "Hide";
            }
            else
            {
                records.Content = db.Phones
                            .Where(x => (x.Inactive == false) && filter == null || (x.Model.Contains(filter)) || x.Company.Contains(filter))
                            .OrderBy(sort + " " + sortdir)
                            .Skip((page - 1) * pageSize)
                            .Take(pageSize)
                            .ToList();
                ViewBag.showInactive = true;
                ViewBag.labelInactive = "Show";
            }

            // Count
            records.TotalRecords = db.Phones
                         .Where(x => filter == null ||
                               (x.Model.Contains(filter)) || x.Company.Contains(filter)).Count();

            records.CurrentPage = page;
            records.PageSize = pageSize;

                return PartialView("_Grid", records);

        }

        // GET: /Phone/Details/5
        public ActionResult Details(int id = 0)
        {
            Phone phone = db.Phones.Find(id);
            if (phone == null)
            {
                return HttpNotFound();
            }
            return PartialView("Details", phone);
        }


        // GET: /Phone/Create
        [HttpGet]
        public ActionResult Create()
        {
            var phone = new Phone();
            return PartialView("Create", phone);
        }

        // POST: /Phone/Create
        [HttpPost]
        public JsonResult Create(Phone phone)
        {
            if (ModelState.IsValid)
            {
                db.Phones.Add(phone);
                db.SaveChanges();
                return Json(new { success = true, ctrl = "Phone", action = "insert", id = phone.PhoneId,
                    col = new[] {
                        phone.PhoneId.ToString(),
                        phone.Model,
                        phone.Company,
                        string.Format("{0:C}", phone.Price)
                    }
                });
            }
            return Json(phone, JsonRequestBehavior.AllowGet);
        }

        // GET: /Phone/Edit/5
        [HttpGet]
        public ActionResult Edit(int id = 0)
        {
            var phone = db.Phones.Find(id);
            if (phone == null)
            {
                return HttpNotFound();
            }

            return PartialView("Edit", phone);
        }

        // POST: /Phone/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Phone phone)
        {
            if (ModelState.IsValid)
            {
                db.Entry(phone).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true, action = "edit", inactive = phone.Inactive.ToString(),
                    col = new[] {
                        phone.PhoneId.ToString(),
                        phone.Model,
                        phone.Company,
                        string.Format("{0:C}", phone.Price)
                    }
                });
            }
            return PartialView("Edit", phone);
        }

        //
        // GET: /Phone/Delete/5
        public ActionResult Delete(int id = 0)
        {
            Phone phone = db.Phones.Find(id);
            if (phone == null)
            {
                return HttpNotFound();
            }
            return PartialView("Delete", phone);
        }


        //
        // POST: /Phone/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var phone = db.Phones.Find(id);
            db.Phones.Remove(phone);
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