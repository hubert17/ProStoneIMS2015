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
    public class PaymentController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/Payment
        public IQueryable<QuotePayment> GetQuotePayments(int QuoteId)
        {
            return db.QuotePayments.Where(x=>x.QuoteId == QuoteId);
        }

        // GET: api/Payment/5
        [ResponseType(typeof(QuotePayment))]
        public IHttpActionResult GetQuotePayment(int id)
        {
            QuotePayment quotePayment = db.QuotePayments.Find(id);
            if (quotePayment == null)
            {
                return NotFound();
            }

            return Ok(quotePayment);
        }

        // PUT: api/Payment/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutQuotePayment(int id, QuotePayment quotePayment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != quotePayment.Id)
            {
                return BadRequest();
            }

            db.Entry(quotePayment).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!QuotePaymentExists(id))
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

        // POST: api/Payment
        [ResponseType(typeof(QuotePayment))]
        public IHttpActionResult PostQuotePayment(QuotePayment quotePayment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.QuotePayments.Add(quotePayment);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = quotePayment.Id }, quotePayment);
        }

        // DELETE: api/Payment/5
        [ResponseType(typeof(QuotePayment))]
        public IHttpActionResult DeleteQuotePayment(int id)
        {
            QuotePayment quotePayment = db.QuotePayments.Find(id);
            if (quotePayment == null)
            {
                return NotFound();
            }

            db.QuotePayments.Remove(quotePayment);
            db.SaveChanges();

            return Ok(quotePayment);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool QuotePaymentExists(int id)
        {
            return db.QuotePayments.Count(e => e.Id == id) > 0;
        }
    }
}