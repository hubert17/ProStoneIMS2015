using System.Linq;
using System.Web.Mvc;
using System.Linq.Dynamic;
using ProStoneIMS2015.Models;
using System.Data.Entity;

namespace ProStoneIMS2015.Controllers
{
    //[Authorize(Roles = "admin")]
    [Authorize]
    public class UserController : Controller
    {
        private ApplicationDbContext  db = new ApplicationDbContext();


        // GET: /ApplicationUser/
        public ActionResult Index(string filter = null, int page = 1, int pageSize = 5, string sort = "Id", string sortdir = "DESC", bool showInactive = false)
        {

            var records = new PagedList<ApplicationUser>();
            ViewBag.filter = filter;
            //if (!string.IsNullOrEmpty(filter))
            //{
            //    pageSize = 50;
            //}

            if (showInactive)
            {
                records.Content = db.Users
                            .Where( x => filter == null || (x.Email.Contains(filter)) || x.FirstName.Contains(filter) || x.LastName.Contains(filter))
                            .OrderBy(sort + " " + sortdir)
                            .Skip((page - 1) * pageSize)
                            .Take(pageSize)
                            .ToList();
                ViewBag.showInactive = false;
                ViewBag.labelInactive = "Hide";
            }
            else
            {
                records.Content = db.Users
                            .Where(x => (x.Inactive == false) && filter == null || (x.Email.Contains(filter)) || x.FirstName.Contains(filter) || x.LastName.Contains(filter))
                            .OrderBy(sort + " " + sortdir)
                            .Skip((page - 1) * pageSize)
                            .Take(pageSize)
                            .ToList();
                ViewBag.showInactive = true;
                ViewBag.labelInactive = "Show";
            }

            // Count
            records.TotalRecords = db.Users
                         .Where(x => filter == null ||
                               (x.Email.Contains(filter)) || x.FirstName.Contains(filter) || x.LastName.Contains(filter)).Count();

            records.CurrentPage = page;
            records.PageSize = pageSize;

            if (Request.IsAjaxRequest() || Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return PartialView("_Grid", records);
            else
                return View(records);

        }

        public ActionResult IndexAjax(string filter = null, int page = 1, int pageSize = 5, string sort = "Id", string sortdir = "DESC", bool showInactive = false)
        {

            var records = new PagedList<ApplicationUser>();
            ViewBag.filter = filter;
            if (showInactive)
            {
                records.Content = db.Users
                            .Where(x => filter == null || (x.Email.Contains(filter)) || x.FirstName.Contains(filter) || x.LastName.Contains(filter))
                            .OrderBy(sort + " " + sortdir)
                            .Skip((page - 1) * pageSize)
                            .Take(pageSize)
                            .ToList();
                ViewBag.showInactive = false;
                ViewBag.labelInactive = "Hide";
            }
            else
            {
                records.Content = db.Users
                            .Where(x => (x.Inactive == false) && filter == null || (x.Email.Contains(filter)) || x.FirstName.Contains(filter) || x.LastName.Contains(filter))
                            .OrderBy(sort + " " + sortdir)
                            .Skip((page - 1) * pageSize)
                            .Take(pageSize)
                            .ToList();
                ViewBag.showInactive = true;
                ViewBag.labelInactive = "Show";
            }

            // Count
            records.TotalRecords = db.Users
                         .Where(x => filter == null ||
                               (x.Email.Contains(filter)) || x.FirstName.Contains(filter) || x.LastName.Contains(filter)).Count();

            records.CurrentPage = page;
            records.PageSize = pageSize;

                return PartialView("_Grid", records);

        }

        // GET: /ApplicationUser/Details/5
        public ActionResult Details(int id = 0)
        {
            ApplicationUser user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return PartialView("Details", user);
        }


        // GET: /ApplicationUser/Edit/5
        [HttpGet]
        public ActionResult Edit(int id = 0)
        {
            var user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }

            return PartialView("Edit", user);
        }

        // POST: /ApplicationUser/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ApplicationUser user)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true, action = "edit", inactive = user.Inactive.ToString(),
                    col = new[] {
                        user.Id,
                        user.Email,
                        user.FirstName,
                        user.LastName,
                    }
                });
            }
            return PartialView("Edit", user);
        }

        //
        // GET: /ApplicationUser/Delete/5
        public ActionResult Delete(int id = 0)
        {
            ApplicationUser user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return PartialView("Delete", user);
        }


        //
        // POST: /ApplicationUser/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var user = db.Users.Find(id);
            db.Users.Remove(user);
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