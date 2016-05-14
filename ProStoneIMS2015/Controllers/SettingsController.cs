using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ProStoneIMS2015.Models;

namespace ProStoneIMS2015.Controllers
{
    public class SettingsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        private int getTenantId()
        {
            var identity = System.Threading.Thread.CurrentPrincipal.Identity as System.Security.Claims.ClaimsIdentity;
            if (identity != null)
            {
                var userClaim = identity.FindFirst("TenantId");
                if (userClaim != null)
                {
                    return int.Parse(userClaim.Value);
                }
            }
            return 0;
        }


        // GET: Settings/Edit/5
        // GET: /Subscriber/Edit/5
        [Authorize(Roles = "subscriber")]
        [HttpGet]
        public ActionResult Index()
        {
            var subscriber = db.Subscribers.Find(getTenantId());
            if (subscriber == null)
            {
                return HttpNotFound();
            }

            return View(subscriber);
        }

        // POST: /Subscriber/Edit/5
        [Authorize(Roles = "subscriber")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(Subscriber subscriber)
        {
            if (subscriber.TenantId != getTenantId())
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Bad Request");
            }

            var excluded = new[] { "SubscriberKey", "FirstName", "LastName", "Phone", "Email", "AltEmail", "MembershipDate", "Inactive" };
            foreach (var col in excluded)
            {
                ModelState.Remove(col);
            }

            if (ModelState.IsValid)
            {
                var quoteInDB = db.Subscribers.Single(d => d.TenantId == subscriber.TenantId);
                var entry = db.Entry(quoteInDB);
                entry.CurrentValues.SetValues(subscriber);

                foreach (var name in excluded)
                {
                    entry.Property(name).IsModified = false;
                }

                db.SaveChanges();
            }
            return View(subscriber);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
