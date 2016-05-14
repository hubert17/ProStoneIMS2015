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
    public class MeasureController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public object GetMeasureLookup(bool? Lookup)
        {
            return db.Measurements.ToArray().Select(s => new
            {
                MeasureName = s.MeasureName,
            })
            .OrderBy(x => x.MeasureName);
        }

        // GET: api/Measure
        public IQueryable<QuoteMeasure> GetQuoteMeasures(int? QuoteStoneId, int QuoteId)
        {
            return db.QuoteMeasures.Where(x => x.QuoteId == QuoteId);
        }

        // GET: api/Measure?Quoteid
        public IQueryable<QuoteMeasure> GetQuoteMeasuresByQuote(int QuoteId)
        {
            return db.QuoteMeasures.Where(x => x.QuoteId == QuoteId);
        }

        // GET: api/Measure/5
        [ResponseType(typeof(QuoteMeasure))]
        public IHttpActionResult GetQuoteMeasure(int id)
        {
            QuoteMeasure quoteMeasure = db.QuoteMeasures.Find(id);
            if (quoteMeasure == null)
            {
                return NotFound();
            }

            return Ok(quoteMeasure);
        }

        // PUT: api/Measure/5
        [ResponseType(typeof(QuoteMeasure))]
        public IHttpActionResult PutQuoteMeasure(int id, QuoteMeasure quoteMeasure)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != quoteMeasure.Id)
            {
                return BadRequest();
            }

            db.Entry(quoteMeasure).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
                //return CreatedAtRoute("DefaultApi", new { id = quoteMeasure.Id }, quoteMeasure);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!QuoteMeasureExists(id))
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

        // POST: api/Measure
        [ResponseType(typeof(QuoteMeasure))]
        public IHttpActionResult PostQuoteMeasure(QuoteMeasure quoteMeasure)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.QuoteMeasures.Add(quoteMeasure);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = quoteMeasure.Id }, quoteMeasure);
        }

        // DELETE: api/Measure/5
        [ResponseType(typeof(QuoteMeasure))]
        public IHttpActionResult DeleteQuoteMeasure(int id)
        {
            QuoteMeasure quoteMeasure = db.QuoteMeasures.Find(id);
            if (quoteMeasure == null)
            {
                return NotFound();
            }

            db.QuoteMeasures.Remove(quoteMeasure);
            db.SaveChanges();

            return Ok(quoteMeasure);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool QuoteMeasureExists(int id)
        {
            return db.QuoteMeasures.Count(e => e.Id == id) > 0;
        }
    }
}