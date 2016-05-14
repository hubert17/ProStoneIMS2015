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
    public class ServiceController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/Service
        public object GetQuoteServices(int QuoteId)
        {
            var serviceData = from q in db.QuoteServices
                           join t in db.Jservices
                           on q.ServiceId equals t.Id into qt
                              from t in qt.DefaultIfEmpty()
                              where q.QuoteId == QuoteId
                           select new 
                           {
                               Id = q.Id,                               
                               ServiceId = q.ServiceId,
                               ServiceCode = t.ServiceCode,
                               ServiceName = t.ServiceName,
                               Quantity = q.Quantity,
                               Price = q.Price,
                               QuoteId = q.QuoteId
                           };

            var serviceLookup = db.Jservices.ToArray().Select(s =>
                new { Id = s.Id, Name = s.ServiceName + " " + s.Price.ToString("c"), Price = s.Price, ServiceCode = s.ServiceCode });

            var serviceEdge = from q in db.QuoteStones
                                join t in db.Edges
                                on q.EdgeId equals t.Id
                                where q.QuoteId == QuoteId
                                select new
                                {
                                    Id = -1,
                                    ServiceId = q.Id,
                                    ServiceName = "[Edge] " + t.EdgeName,
                                    Quantity = 1,
                                    Price = t.Price,
                                    QuoteId = q.QuoteId
                                };
            //IQueryable<object> serviceData = serviceItem.Cast<object>().Concat(serviceEdge.Cast<object>());
            return new { serviceData, serviceLookup, serviceEdge };
        }

        // GET: api/Service/5
        public object GetQuoteService(int id)
        {
            var serviceData = from q in db.QuoteServices
                           join t in db.Jservices
                           on q.ServiceId equals t.Id into qt
                              from t in qt.DefaultIfEmpty()
                              where q.Id == id
                           select new
                           {
                               Id = q.Id,                               
                               ServiceId = q.ServiceId,
                               ServiceCode = t.ServiceCode,
                               ServiceName = t.ServiceName,
                               Quantity = q.Quantity,
                               Price = t.Price,
                               QuoteId = q.QuoteId
                           };
            return serviceData.FirstOrDefault();
        }

        // PUT: api/Service/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutQuoteService(int id, QuoteService quoteService)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != quoteService.Id)
            {
                return BadRequest();
            }

            db.Entry(quoteService).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
                var serviceData = from q in db.QuoteServices
                               join t in db.Jservices
                               on q.ServiceId equals t.Id into qt
                                  from t in qt.DefaultIfEmpty()
                                  where q.Id == quoteService.Id
                               select new
                               {
                                   Id = q.Id,                                   
                                   ServiceId = q.ServiceId,
                                   ServiceCode = t.ServiceCode,
                                   ServiceName = t.ServiceName,
                                   Quantity = q.Quantity,
                                   Price = t.Price,
                                   QuoteId = q.QuoteId
                                   //Id = q.Id,
                                   //CatalogID = "t.CatalogID",
                                   //ServiceId = q.ServiceId,
                                   //ServiceName = "t.ServiceName",
                                   //Quantity = q.Quantity,
                                   //Price = t.Price,
                                   //QuoteId = q.QuoteId
                               };
                return CreatedAtRoute("DefaultApi", new { id = quoteService.Id }, serviceData.FirstOrDefault());
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!QuoteServiceExists(id))
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

        // POST: api/Service
        [ResponseType(typeof(QuoteService))]
        public IHttpActionResult PostQuoteService(QuoteService quoteService)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.QuoteServices.Add(quoteService);
            db.SaveChanges();

            var serviceData = from q in db.QuoteServices
                           join t in db.Jservices
                           on q.ServiceId equals t.Id
                           where q.Id == quoteService.Id
                           select new
                           {
                               Id = q.Id,                               
                               ServiceId = q.ServiceId,
                               ServiceCode = t.ServiceCode,
                               ServiceName = t.ServiceName,
                               Quantity = q.Quantity,
                               Price = t.Price,
                               QuoteId = q.QuoteId
                           };

            return CreatedAtRoute("DefaultApi", new { id = quoteService.Id }, serviceData.FirstOrDefault());
        }

        // DELETE: api/Service/5
        [ResponseType(typeof(QuoteService))]
        public IHttpActionResult DeleteQuoteService(int id)
        {
            QuoteService quoteService = db.QuoteServices.Find(id);
            if (quoteService == null)
            {
                return NotFound();
            }

            db.QuoteServices.Remove(quoteService);
            db.SaveChanges();

            return Ok(quoteService);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool QuoteServiceExists(int id)
        {
            return db.QuoteServices.Count(e => e.Id == id) > 0;
        }
    }
}