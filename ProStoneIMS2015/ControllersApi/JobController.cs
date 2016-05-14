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
using System.Web;

namespace ProStoneIMS2015.ControllersApi
{
    [Authorize]
    public class JobController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/Job
        public IHttpActionResult GetQuotes()
        {
            var PayStatus = db.PayStatuss.Select(s => new { value = s.Code, text = s.Name });
            var Backsplash = db.Backsplashs.Select(s => new { value = s.Id, text = s.Name });
            var Lead = db.Leads.Where(x => x.Inactive == false).Select(s => new { value = s.Id, text = s.LeadName }).OrderBy(x => x.text);
            var Salesman = db.Salesmans.Where(x => x.Inactive == false).Select(s => new { value = s.Id, text = s.SalesmanName }).OrderBy(x => x.text);
            var TaskStatus = db.TaskStatuss.Select(s => new { value = s.Code, text = s.Name }).OrderBy(x => x.text);
            var AssignedTo = db.Assignedtos.Where(x => x.Inactive == false).Select(s => new { value = s.Id, text = s.AssignedtoName }).OrderBy(x => x.text);


            return Ok(new { PayStatus, Backsplash, Lead, Salesman, TaskStatus, AssignedTo });
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

            var quoteInDB = db.Quotes.Single(d => d.Id == quote.Id);
            var entry = db.Entry(quoteInDB);
            entry.CurrentValues.SetValues(quote);

            var excluded = new[] { "LastName", "FirstName", "Address", "City", "State", "Zip", "Phone", "Email" };
            foreach (var name in excluded)
            {
                entry.Property(name).IsModified = false;
            }

            try
            {
                var ovTaskStatus = entry.OriginalValues["TaskStatus"] != null ? entry.OriginalValues["TaskStatus"].ToString() : string.Empty;
                var nvTaskStatus = entry.CurrentValues["TaskStatus"] != null ? entry.CurrentValues["TaskStatus"].ToString() : string.Empty;
                var ovTaskDate = entry.OriginalValues["TaskDate"] != null ? entry.OriginalValues["TaskDate"].ToString() : string.Empty;
                var nvTaskDate = entry.CurrentValues["TaskDate"] != null ? entry.CurrentValues["TaskDate"].ToString() : string.Empty;
                //var ovTaskTime = entry.OriginalValues["TaskTime"] != null ? entry.OriginalValues["TaskTaskTime"].ToString() : string.Empty;
                //var nvTaskTime = entry.CurrentValues["TaskTime"] != null ? entry.CurrentValues["TaskTaskTime"].ToString() : string.Empty;
                var ovAssignedTo = entry.OriginalValues["AssignedTo"] != null ? entry.OriginalValues["AssignedTo"].ToString() : string.Empty;
                var nvAssignedTo = entry.CurrentValues["AssignedTo"] != null ? entry.CurrentValues["AssignedTo"].ToString() : string.Empty;

                if (!ovTaskStatus.Equals(nvTaskStatus) || ovTaskDate != nvTaskDate || /*ovTaskTime != nvTaskTime || */ ovAssignedTo != nvAssignedTo)
                {
                    var quoteHistory = new QuoteHistory
                    {
                        TaskStatus = quote.TaskStatus,
                        TaskDate = quote.TaskDate,
                        TaskTime = quote.TaskTime,
                        AssignedToId = quote.AssignedTo,
                        QuoteId = quote.Id
                    };
                    db.Set<QuoteHistory>().Add(quoteHistory);
                }
            }
            catch { throw; }

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

        [HttpPost]
        public HttpResponseMessage UploadFile(int QuoteId, string SendAs)
        {
            HttpResponseMessage result = null;
            var httpRequest = HttpContext.Current.Request;

            // Check if files are available
            if (httpRequest.Files.Count > 0)
            {
                // interate the files and save on the server
                var postedFile = httpRequest.Files["UploadedFile"];
                var newFilename = QuoteId.ToString() + "_" + SendAs + ".pdf";
                var filePath = HttpContext.Current.Server.MapPath("~/Attachment/" + newFilename);
                postedFile.SaveAs(filePath);

                // return result
                result = Request.CreateResponse(HttpStatusCode.Created, newFilename);
            }
            else
            {
                // return BadRequest (no file(s) available)
                result = Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            return result;
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