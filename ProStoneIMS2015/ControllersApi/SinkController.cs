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
    public class SinkController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/Sink
        public object GetQuoteSinks(int QuoteId)
        {
            //var sinkData = db.QuoteSinks.Where(x => x.QuoteId == QuoteId);
            var sinkData = from q in db.QuoteSinks
                        join t in db.Sinks
                        on q.SinkId equals t.Id into qt
                           from t in qt.DefaultIfEmpty()
                           where q.QuoteId == QuoteId
                        select new
                        {
                            Id = q.Id,
                            CatalogID = t.CatalogID,
                            SinkId = q.SinkId,
                            SinkName = t.SinkName,
                            Quantity = q.Quantity,
                            Price = q.Price,
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

        public object GetSinkInventory(int sinkId)
        {
            var inventory = from s in db.SinkInventorys
                            where s.Inactive == false && s.SinkId == sinkId
                            join t in db.Sinks
                            on s.SinkId equals t.Id into st
                            from t in st.DefaultIfEmpty()
                            join v in db.Vendors
                            on s.VendorId equals v.Id into sv
                            from v in sv.DefaultIfEmpty()
                            select new
                            {
                                Id = s.Id,
                                SinkId = s.SinkId,
                                SinkName = t.SinkName,
                                SerialNo = s.SerialNo,
                                UnitPrice = s.UnitPrice,
                                SalesTax = s.SalesTax,
                                Total = s.UnitPrice + s.UnitPrice * ((s.SalesTax / 100) ?? 0),
                                VendorName = v.VendorName,
                                DateSold = s.DateSold,
                                QuoteId = s.QuoteId,
                                Inactive = s.Inactive
                            };


            return inventory;
        }

        public object GetHoldInventory(int QId)
        {
            var inventory = from s in db.SinkInventorys
                            where s.Inactive == false && s.QuoteId == QId 
                            join t in db.Sinks
                            on s.SinkId equals t.Id into st
                            from t in st.DefaultIfEmpty()
                            join v in db.Vendors
                            on s.VendorId equals v.Id into sv
                            from v in sv.DefaultIfEmpty()
                            select new
                            {
                                Id = s.Id,
                                SinkId = s.SinkId,
                                SinkName = t.SinkName,
                                SerialNo = s.SerialNo,
                                UnitPrice = s.UnitPrice,
                                SalesTax = s.SalesTax,
                                Total = s.UnitPrice + s.UnitPrice * ((s.SalesTax / 100) ?? 0),
                                VendorName = v.VendorName,
                                DateSold = s.DateSold,
                                QuoteId = s.QuoteId,
                                Inactive = s.Inactive
                            };


            return inventory;
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

            //return StatusCode(HttpStatusCode.NoContent);
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

        public IHttpActionResult PutInventory(int Id, int? QuoteId)
        {
            //var inventory = db.StoneInventorys.Where(o => o.Id == Id).Single();
            var inventory = db.SinkInventorys.Find(Id);

            try
            {
                if (inventory.QuoteId == null)
                {
                    inventory.QuoteId = QuoteId;
                }
                else
                {
                    inventory.QuoteId = null;
                }

                db.SaveChanges();
            }
            catch
            {
                throw;
            }

            return CreatedAtRoute("DefaultApi", new { id = Id }, inventory);
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