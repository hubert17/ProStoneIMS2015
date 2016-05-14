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
    public class SinkController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Sink
        // GET: /Sink/
        public ActionResult Index(string filter = null, int page = 1, int pageSize = 5, string sort = "Id", string sortdir = "DESC", bool showInactive = false)
        {

            var records = new PagedList<Sink>();
            ViewBag.filter = filter;

            var qry = from s in db.Sinks
                      join t in db.Sink_types
                      on s.Type equals t.Id into st
                      from t in st.DefaultIfEmpty()
                      select new
                      {
                          Id = s.Id,
                          SinkName = s.SinkName,
                          CatalogID = s.CatalogID,
                          Price = s.Price,
                          TypeName = t.Name,
                          Inactive = s.Inactive
                      };

            if (showInactive)
            {
                records.Content = qry
                            .Where(x => filter == null || x.SinkName.Contains(filter))
                            .OrderBy(sort + " " + sortdir)
                            .Skip((page - 1) * pageSize)
                            .Take(pageSize)
                            .ToList()
                                .Select(x => new Sink()
                                {
                                    Id = x.Id,
                                    SinkName = x.SinkName,
                                    CatalogID = x.CatalogID,
                                    Price = x.Price,
                                    TypeName = x.TypeName,
                                    Inactive = x.Inactive
                                }).ToList();
                ViewBag.showInactive = false;
                ViewBag.labelInactive = "Hide";
            }
            else
            {
                records.Content = qry
                            .Where(x => (x.Inactive == false) && filter == null || x.SinkName.Contains(filter))
                            .OrderBy(sort + " " + sortdir)
                            .Skip((page - 1) * pageSize)
                            .Take(pageSize)
                            .ToList()
                                .Select(x => new Sink()
                                {
                                    Id = x.Id,
                                    SinkName = x.SinkName,
                                    CatalogID = x.CatalogID,
                                    Price = x.Price,
                                    TypeName = x.TypeName,
                                    Inactive = x.Inactive
                                }).ToList();
                ViewBag.showInactive = true;
                ViewBag.labelInactive = "Show";
            }

            // Count
            records.TotalRecords = db.Sinks
                         .Where(x => filter == null || x.SinkName.Contains(filter)).Count();

            records.CurrentPage = page;
            records.PageSize = pageSize;

            if (Request.IsAjaxRequest() || Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return PartialView("_Grid", records);
            else
                return View(records);

        }

        public ActionResult Details(int id = 0)
        {
            Sink sink = db.Sinks.Find(id);
            if (sink == null)
            {
                return HttpNotFound();
            }
            return PartialView("Details", sink);
        }

        private IEnumerable<SelectListItem> GetSinkType()
        {
            var record = db.Sink_types
                .Where(x => x.Inactive == false)
                .Select(s => new SelectListItem { Value = s.Id.ToString(), Text = s.Name });

            return new SelectList(record, "Value", "Text");
        }

        // GET: /Sink/Create
        [HttpGet]
        public ActionResult Create()
        {
            var sinkVM = new SinkVM();
            sinkVM.sink = new Sink();
            sinkVM.sink_type = GetSinkType();
            return PartialView("Create", sinkVM);
        }

        // POST: /Sink/Create
        [HttpPost]
        public JsonResult Create(SinkVM sinkvm)
        {
            if (ModelState.IsValid)
            {
                db.Sinks.Add(sinkvm.sink);
                db.SaveChanges();
                return Json(new
                {
                    success = true,
                    ctrl = "Sink",
                    action = "insert",
                    id = sinkvm.sink.Id.ToString(),
                    col = new[] {
                        sinkvm.sink.CatalogID,
                        sinkvm.sink.SinkName,
                        string.Format("{0:C}", sinkvm.sink.Price),
                        db.Sink_types.Where(x => x.Id == sinkvm.sink.Type).Select(s => s.Name).FirstOrDefault().ToString(),
                        sinkvm.sink.ImageFilename,
                    }
                });
            }
            return Json(sinkvm.sink, JsonRequestBehavior.AllowGet);
        }

        // GET: /Sink/Edit/5
        [HttpGet]
        public ActionResult Edit(int id = 0)
        {
            var sinkVM = new SinkVM();
            sinkVM.sink = db.Sinks.Find(id);
            sinkVM.sink_type = GetSinkType();

            if (sinkVM.sink == null)
            {
                return HttpNotFound();
            }

            return PartialView("Edit", sinkVM);
        }

        // POST: /Sink/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(SinkVM sinkvm)
        {
            var sink = sinkvm.sink;
            if (ModelState.IsValid)
            {
                db.Entry(sink).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new
                {
                    success = true,
                    action = "edit",
                    inactive = sink.Inactive.ToString(),
                    col = new[] {
                        sink.CatalogID,
                        sink.SinkName,
                        string.Format("{0:C}", sink.Price),
                        db.Sink_types.Where(x => x.Id == sink.Type).Select(s => s.Name).FirstOrDefault().ToString(),
                        sink.ImageFilename
                    }
                });
            }
            return PartialView("Edit", sink);
        }

        //
        // GET: /Sink/Delete/5
        public ActionResult Delete(int id = 0)
        {
            Sink sink = db.Sinks.Find(id);
            if (sink == null)
            {
                return HttpNotFound();
            }
            return PartialView("Delete", sink);
        }


        //
        // POST: /Sink/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var sink = db.Sinks.Find(id);
            db.Sinks.Remove(sink);
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