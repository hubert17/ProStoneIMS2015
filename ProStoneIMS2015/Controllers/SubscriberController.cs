using System.Linq;
using System.Web.Mvc;
using System.Linq.Dynamic;
using ProStoneIMS2015.Models;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace ProStoneIMS2015.Controllers
{
    [Authorize(Roles = "admin")]
    public class SubscriberController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Subscriber
        [AllowAnonymous]
        public ActionResult Index(string filter = null, int page = 1, int pageSize = 5, string sort = "TenantId", string sortdir = "DESC", bool showInactive = false)
        {
            if(!(Request.IsAuthenticated && User.IsInRole("admin")))
            {
                return RedirectToAction("SubscriptionSignUp", "Account");
            }

            var records = new PagedList<Subscriber>();
            ViewBag.filter = filter;

            if (showInactive)
            {
                records.Content = db.Subscribers
                            .Where(x => filter == null || (x.CompanyName.Contains(filter)) || x.City.Contains(filter) || x.State.Contains(filter))
                            .OrderBy(sort + " " + sortdir)
                            .Skip((page - 1) * pageSize)
                            .Take(pageSize)
                            .ToList();
                ViewBag.showInactive = false;
                ViewBag.labelInactive = "Hide";
            }
            else
            {
                records.Content = db.Subscribers
                            .Where(x => (x.Inactive == false) && filter == null || (x.CompanyName.Contains(filter)) || x.City.Contains(filter) || x.State.Contains(filter))
                            .OrderBy(sort + " " + sortdir)
                            .Skip((page - 1) * pageSize)
                            .Take(pageSize)
                            .ToList();
                ViewBag.showInactive = true;
                ViewBag.labelInactive = "Show";
            }

            // Count
            records.TotalRecords = db.Subscribers
                         .Where(x => filter == null ||
                               (x.CompanyName.Contains(filter)) || x.City.Contains(filter) || x.State.Contains(filter)).Count();

            records.CurrentPage = page;
            records.PageSize = pageSize;

            if (Request.IsAjaxRequest() || Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return PartialView("_Grid", records);
            else
                return View(records);
        }

        // GET: /Subscriber/Details/5
        [Authorize(Roles = "admin")]
        public ActionResult Details(int id = 0)
        {
            Subscriber subscriber = db.Subscribers.Find(id);
            if (subscriber == null)
            {
                return HttpNotFound();
            }
            return PartialView("Details", subscriber);
        }

        // GET: /Subscriber/Edit/5
        [Authorize(Roles = "admin")]
        [HttpGet]
        public ActionResult Edit(int id = 0)
        {
            var subscriber = db.Subscribers.Find(id);
            if (subscriber == null)
            {
                return HttpNotFound();
            }

            return PartialView("Edit", subscriber);
        }

        // POST: /Subscriber/Edit/5
        [Authorize(Roles = "admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Subscriber subscriber)
        {
            if (ModelState.IsValid)
            {
                db.Entry(subscriber).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new
                {
                    success = true,
                    action = "edit",
                    col = new[] {
                        subscriber.TenantId.ToString(),
                        subscriber.CompanyName,
                        subscriber.Address,
                        subscriber.City,
                        subscriber.State
                    }
                });
            }
            return PartialView("Edit", subscriber);
        }

        //
        // GET: /Subscriber/Delete/5
        [Authorize(Roles = "admin")]
        public ActionResult Delete(int id = 0)
        {
            Subscriber subscriber = db.Subscribers.Find(id);
            if (subscriber == null)
            {
                return HttpNotFound();
            }
            return PartialView("Delete", subscriber);
        }


        //
        // POST: /Subscriber/Delete/5
        [Authorize(Roles = "admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var subscriber = db.Subscribers.Find(id);
            db.Subscribers.Remove(subscriber);
            db.SaveChanges();
            return Json(new { success = true, action = "delete" });
        }

        // GET: /subscriber/signup
        [AllowAnonymous]
        [HttpGet]
        public ActionResult SignUp()
        {
            return RedirectToAction("SubscriptionSignUp", "Account");
        }
       
        [AllowAnonymous]
        public JsonResult IsKeyExists(string subscriberKey)
        {
            //check if any of the UserName matches the UserName specified in the Parameter using the ANY extension method.  
            string[] subKeys = System.Web.HttpRuntime.Cache["subscriberKey"] as string[];
            return Json(!subKeys.Contains(subscriberKey), JsonRequestBehavior.AllowGet);

        }

        //check if any of the UserName matches the UserName specified in the Parameter using the ANY extension method.  
        [AllowAnonymous]
        public JsonResult CheckSubscriberKey(string subscriberKey)
        {
            //return Json(db.Subscribers.Any(x => x.SubscriberKey == subscriberKey), JsonRequestBehavior.AllowGet);
            string[] subKeys = System.Web.HttpRuntime.Cache["subscriberKey"] as string[];
            return Json(subKeys.Contains(subscriberKey), JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

    }
}