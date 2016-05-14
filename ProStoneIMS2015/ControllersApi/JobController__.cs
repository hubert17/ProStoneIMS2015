using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using ProStoneIMS2015.Models;

namespace ProStoneIMS2015.ControllersApi
{
    public class JobController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/Job
        public IHttpActionResult GetQuotes()
        {
            var payStatus = db.PayStatuss.Select(s => new { value = s.Code, text = s.Name });
            var backsplash = db.Backsplashs.Select(s => new { value = s.Id, text = s.Name });
            var lead = db.Leads.Where(x => x.Inactive == false).Select(s => new { value = s.Id, text = s.LeadName }).OrderBy(x => x.text);
            var salesman = db.Salesmans.Where(x => x.Inactive == false).Select(s => new { value = s.Id, text = s.SalesmanName }).OrderBy(x => x.text);
            var taskstatus = db.TaskStatuss.Select(s => new { value = s.Code, text = s.Name }).OrderBy(x => x.text);
            var assignedto = db.Assignedtos.Where(x => x.Inactive == false).Select(s => new { value = s.Id, text = s.AssignedtoName }).OrderBy(x => x.text);

            return Ok(new { payStatus, backsplash, lead, salesman, taskstatus, assignedto });
        }

        // GET: api/Job/5
        [ResponseType(typeof(Quote))]
        public async Task<IHttpActionResult> GetQuote(int id)
        {
            var quote = await (from q in db.Quotes
                               join p in db.PayStatuss on q.PayStatus equals p.Code into qp
                               from p in qp.DefaultIfEmpty()
                               join b in db.Backsplashs on q.Backsplash equals b.Id into qb
                               from b in qb.DefaultIfEmpty()
                               join l in db.Leads on q.Lead equals l.Id into ql
                               from l in ql.DefaultIfEmpty()
                               join s in db.Salesmans on q.Salesman equals s.Id into qs
                               from s in qs.DefaultIfEmpty()
                               join t in db.TaskStatuss on q.TaskStatus equals t.Code into qt
                               from t in qt.DefaultIfEmpty()
                               join a in db.Assignedtos on q.AssignedTo equals a.Id into qa
                               from a in qa.DefaultIfEmpty()
                               where q.Id == id
                                    select new
                                    {
                                        Id = q.Id,
                                        JobNo = q.JobNo,
                                        Verified = q.Verified,
                                        PayStatus = q.PayStatus,
                                        PayStatus_ = p.Name,
                                        Backsplash = q.Backsplash,
                                        Backsplash_ = b.Name,
                                        Lead = q.Lead,
                                        Lead_ = l.LeadName,
                                        DateCreated = q.DateCreated,
                                        Salesman = q.Salesman,
                                        Salesman_ = s.SalesmanName,
                                        TaskDate = q.TaskDate,
                                        TaskTime = q.TaskTime,
                                        TaskStatus = q.TaskStatus,
                                        TaskStatus_ = t.Name,
                                        AssignedTo = q.AssignedTo,
                                        AssignedTo_ = a.AssignedtoName,

                                        LastName = q.LastName,
                                        FirstName = q.FirstName,
                                        Address = q.Address,
                                        City = q.City,
                                        State = q.State,
                                        Zip = q.Zip,
                                        Phone = q.Phone,
                                        Email = q.Email,

                                    }).FirstOrDefaultAsync();


            if (quote == null)
            {
                return NotFound();
            }

            return Ok(quote);
        }

        //PUT: api/Job/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutQuote(int id, Quote quote)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != quote.Id)
            {
                return BadRequest();
            }

            db.Entry(quote).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!QuoteExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }


        //POST: api/Job
       [ResponseType(typeof(Quote))]
        public async Task<IHttpActionResult> PostQuote(Quote quote)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Quotes.Add(quote);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = quote.Id }, quote);
        }

        // DELETE: api/Job/5
        [ResponseType(typeof(Quote))]
        public async Task<IHttpActionResult> DeleteQuote(int id)
        {
            Quote quote = await db.Quotes.FindAsync(id);
            if (quote == null)
            {
                return NotFound();
            }

            db.Quotes.Remove(quote);
            await db.SaveChangesAsync();

            return Ok(quote);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool QuoteExists(int id)
        {
            return db.Quotes.Count(e => e.Id == id) > 0;
        }
    }
}