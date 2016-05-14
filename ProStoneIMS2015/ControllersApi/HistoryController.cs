using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using ProStoneIMS2015.Models;

namespace ProStoneIMS2015.ControllersApi
{
    public class HistoryController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/History
        public IQueryable<QuoteHistory> GetQuoteHistorys()
        {
            //return db.QuoteHistorys;
            return null;
        }

        // GET: api/History/5
        [ResponseType(typeof(QuoteHistory))]
        public IHttpActionResult GetQuoteHistory(int id)
        {
            var maxHistory = db.QuoteHistorys.Where(x => x.QuoteId == id).Max(e => e.Id);
            if (db.QuoteHistorys.Where(x => x.QuoteId == id).Count() <= 1)
               maxHistory = -1;
                  
            var quoteHistory = from qh in db.QuoteHistorys
                                join ts in db.TaskStatuss on qh.TaskStatus equals ts.Code
                                join at in db.Assignedtos on qh.AssignedToId equals at.Id
                                where qh.QuoteId == id && qh.Id != maxHistory
                                orderby qh.TaskDate
                               select new
                                {
                                    Id = qh.Id,
                                    TaskStatus = ts.Name,
                                    TaskDate = qh.TaskDate,
                                    TaskTime = qh.TaskTime,
                                    AssignedTo = at.AssignedtoName
                                };

            if (quoteHistory == null)
            {
                return NotFound();
            }

            return Ok(quoteHistory);
        }

        // DELETE: api/History/5
        [ResponseType(typeof(QuoteHistory))]
        public IHttpActionResult DeleteQuoteHistory(int id)
        {
            QuoteHistory quoteHistory = db.QuoteHistorys.Find(id);
            if (quoteHistory == null)
            {
                return NotFound();
            }

            db.QuoteHistorys.Remove(quoteHistory);
            db.SaveChanges();

            return Ok(quoteHistory);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool QuoteHistoryExists(int id)
        {
            return db.QuoteHistorys.Count(e => e.Id == id) > 0;
        }
    }
}