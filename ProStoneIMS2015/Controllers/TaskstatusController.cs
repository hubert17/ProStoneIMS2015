using ProStoneIMS2015.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Linq.Dynamic;
using System.Data.Entity;
using System.Text;

namespace ProStoneIMS2015.Controllers
{
    public class TaskstatusController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult Index()
        {
            var fnameLookup = db.Quotes.OrderByDescending(o => o.Id).Take(20).Select(s => s.FirstName + " " + s.LastName);
            ViewBag.Typeahead = string.Join("','", fnameLookup.ToArray());
            ViewBag.Typeahead += "','quote','install";
            return View();
        }

        [HttpGet]
        public JsonResult GetTaskStatus(int page = 1, int limit = 5, string sortBy = "Id", string direction = "desc", string searchString = null, bool showInactive = false)
        {
            var records = new List<Quote>();
            int total = 0;

            var qry = from q in db.Quotes select q;

            if (!String.IsNullOrEmpty(searchString) && searchString.ToLower().Contains("quote"))
            {
                qry = qry.Where(x => x.TaskStatus == "INS");
                searchString = null;
            }

            if (showInactive)
            {
                records = qry
                            .Where(x => searchString == null || (x.FirstName + " " + x.LastName).Contains(searchString))
                            .OrderBy(sortBy + " " + direction)
                            .Skip((page - 1) * limit)
                            .Take(limit)
                            .ToList()
                                .Select(s => new Quote
                                {
                                    Id = s.Id,
                                    LastName = s.LastName,
                                    FirstName = s.FirstName,
                                    Address = s.Address + ", " + s.City + ", " + s.State + ", " + s.Zip,
                                    DateCreated = s.DateCreated,
                                    TaskDate = s.TaskDate,
                                    Inactive = s.Inactive
                                }).ToList();
                ViewBag.showInactive = false;
                ViewBag.labelInactive = "Hide";
                total = qry.Where(x => searchString == null || (x.FirstName + " " + x.LastName).Contains(searchString)).Count();
            }
            else
            {
                records = qry
                            .Where(x => (x.Inactive == false) && searchString == null || (x.FirstName + " " + x.LastName).Contains(searchString))
                            .OrderBy(sortBy + " " + direction)
                            .Skip((page - 1) * limit)
                            .Take(limit)
                            .ToList()
                                .Select(s => new Quote
                                {
                                    Id = s.Id,
                                    LastName = s.LastName,
                                    FirstName = s.FirstName,
                                    Address = s.Address + ", " + s.City + ", " + s.State + ", " + s.Zip,
                                    DateCreated = s.DateCreated,
                                    TaskDate = s.TaskDate,
                                    Inactive = s.Inactive
                                }).ToList();
                ViewBag.showInactive = true;
                ViewBag.labelInactive = "Show";
                total = qry.Where(x => (x.Inactive == false) && searchString == null || (x.FirstName + " " + x.LastName).Contains(searchString)).Count();
            }

            // Count

            return Json(new { records, total }, JsonRequestBehavior.AllowGet);
        }

    }
}
