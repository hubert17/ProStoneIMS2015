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
    [Authorize]
    public class SinkController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/Sink
        public object GetQuoteSinks(int QuoteId)
        {
            var sinkData = from q in db.QuoteSinks
                      join t in db.Sinks
                      on q.SinkId equals t.Id
                      where q.QuoteId == QuoteId
                      select new
                      {
                          Id = q.Id,
                          CatalogID = t.CatalogID,
                          SinkId = q.SinkId,
                          SinkName = t.SinkName,
                          Quantity = q.Quantity,
                          Price = t.Price,
                          QuoteId = q.QuoteId
                      };
            var sinkLookup = db.Sinks.ToArray().Select(s =>
                new { Id = s.Id, Name = s.CatalogID + " - " + s.SinkName + " " + s.Price.ToString("c"), CatalogID = s.CatalogID, Price = s.Price });

            return new { sinkData, sinkLookup };
        }

        // GET: api/Sink/5
        public object GetQuoteSink(int id)
        {
            var sinkData = from q in db.QuoteSinks                           
                           join t in db.Sinks
                           on q.SinkId equals t.Id
                           where q.Id == id
                           select new
                           {
                               Id = q.Id,
                               CatalogID = t.CatalogID,
                               SinkId = q.SinkId,
                               SinkName = t.SinkName,
                               Quantity = q.Quantity,
                               Price = t.Price,
                               QuoteId = q.QuoteId
                           };
            return sinkData.FirstOrDefault();
        }

        // PUT: api/Sink/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutQuoteSink(int id, QuoteSink quoteSink)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != quoteSink.Id)
            {
                return BadRequest();
            }

            db.Entry(quoteSink).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
                var sinkData = from q in db.QuoteSinks
                               join t in db.Sinks
                               on q.SinkId equals t.Id
                               where q.Id == quoteSink.Id
                               select new
                               {
                                   Id = q.Id,
                                   CatalogID = t.CatalogID,
                                   SinkId = q.SinkId,
                                   SinkName = t.SinkName,
                                   Quantity = q.Quantity,
                                   Price = t.Price,
                                   QuoteId = q.QuoteId
                                   //Id = q.Id,
                                   //CatalogID = "t.CatalogID",
                                   //SinkId = q.SinkId,
                                   //SinkName = "t.SinkName",
                                   //Quantity = q.Quantity,
                                   //Price = t.Price,
                                   //QuoteId = q.QuoteId
                               };
                return CreatedAtRoute("DefaultApi", new { id = quoteSink.Id }, sinkData.FirstOrDefault());
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!QuoteSinkExists(id))
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

        // POST: api/Sink
        [ResponseType(typeof(QuoteSink))]
        public IHttpActionResult PostQuoteSink(QuoteSink quoteSink)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.QuoteSinks.Add(quoteSink);
            db.SaveChanges();

            var sinkData = from q in db.QuoteSinks
                           join t in db.Sinks
                           on q.SinkId equals t.Id
                           where q.Id == quoteSink.Id
                           select new
                           {
                               Id = q.Id,
                               CatalogID = t.CatalogID,
                               SinkId = q.SinkId,
                               SinkName = t.SinkName,
                               Quantity = q.Quantity,
                               Price = t.Price,
                               QuoteId = q.QuoteId
                           };

            return CreatedAtRoute("DefaultApi", new { id = quoteSink.Id }, sinkData.FirstOrDefault());
        }

        // DELETE: api/Sink/5
        [ResponseType(typeof(QuoteSink))]
        public IHttpActionResult DeleteQuoteSink(int id)
        {
            QuoteSink quoteSink = db.QuoteSinks.Find(id);
            if (quoteSink == null)
            {
                return NotFound();
            }

            db.QuoteSinks.Remove(quoteSink);
            db.SaveChanges();

            return Ok(quoteSink);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool QuoteSinkExists(int id)
        {
            return db.QuoteSinks.Count(e => e.Id == id) > 0;
        }
    }
}