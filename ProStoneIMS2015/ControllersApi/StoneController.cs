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
    public class StoneController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        private int getTenantId()
        {
            var identity = System.Threading.Thread.CurrentPrincipal.Identity as System.Security.Claims.ClaimsIdentity;
            if (identity != null)
            {
                var userClaim = identity.FindFirst("TenantId");
                if (userClaim != null)
                {
                    return int.Parse(userClaim.Value);
                }
            }
            return 0;
        }

        // GET: api/Stone?Lookup
        public object GetStoneLookup(bool? Lookup)
        {
            var stoneLookup = db.Stones.ToArray().Select(s => new
            {
                Id = s.Id,
                StoneName = s.StoneNameCm,
                PublishedPrice = 0.00,
            })
            .OrderBy(x => x.StoneName);

            var subscriber = db.Subscribers.Find(getTenantId());
            var stoneDefault = new
            {
                StateTax = subscriber.StateTax,
                PlusVal = subscriber.PlusVal,
                FabMin = subscriber.FabMin,
                FabMinPrice = subscriber.FabMinPrice
            };


            var edgeLookup = db.Edges.ToArray().Select(s => new 
            {
                Id = s.Id,
                EdgeName = s.EdgeName + " - " + (s.Price == 0 ? "Free" : string.Format("{0:C}", s.Price)),
                Price = s.Price
            })
            .OrderBy(x => x.EdgeName);

            return new { stoneLookup, stoneDefault, edgeLookup };
        }

        public object GetInventory(int stoneId)
        {
            var inventory = from s in db.StoneInventorys
                      where s.Inactive == false && s.StoneId == stoneId 
                      join t in db.Stones
                      on s.StoneId equals t.Id into st
                      from t in st.DefaultIfEmpty()
                      join v in db.Vendors
                      on s.VendorId equals v.Id into sv
                      from v in sv.DefaultIfEmpty()
                      select new
                      {
                          Id = s.Id,
                          StoneId = s.StoneId,
                          StoneName = t.StoneName + " " + t.Thickness,
                          SerialNo = s.SerialNo,
                          Length = s.Length,
                          Width = s.Width,
                          SF = Math.Round((double)(s.Length * s.Width) / 144.00, 2),
                          UnitPrice = s.UnitPrice,
                          SalesTax = s.SalesTax,
                          Total = (s.UnitPrice + s.UnitPrice * ((s.SalesTax / 100) ?? 0)) * ((double)(s.Length * s.Width) / 144.00),
                          LotNo = s.LotNo,
                          VendorName = v.VendorName,
                          Consignment = s.Consignment,
                          DateSold = s.DateSold,
                          QuoteId = s.QuoteId,
                          Inactive = s.Inactive
                      };


            return inventory;
        }

        // GET: api/Stone
        //[ResponseType(typeof(QuoteStone))]
        //public IHttpActionResult GetQuoteStones(int QuoteId)
        //{
        //    //var quoteStone  = db.QuoteStones.Where(x => x.QuoteId == QuoteId);
        //    var quoteStone = (from q in db.QuoteStones
        //                      where q.QuoteId == QuoteId
        //                      join t in db.Stones
        //                      on q.StoneId equals t.Id
        //                      select new
        //                      {
        //                          Id = q.Id,
        //                          StoneId = q.StoneId,
        //                          //StoneName = t.StoneName + " " + t.Thickness,
        //                          PublishedPrice = q.PublishedPrice,
        //                          Length = q.Length,
        //                          Width = q.Width,
        //                          SquareFeet = q.SquareFeet,
        //                          StateTax = q.StateTax,
        //                          Surcharge = q.Surcharge,
        //                          NSlab = q.NSlab,
        //                          SquareFeetQty = q.SquareFeetQty,
        //                          NSlabSFdiv = q.NSlabSFdiv,
        //                          NSlabSF = q.NSlabSF,
        //                          SF = q.SF,
        //                          PlusVal = q.PlusVal,
        //                          SFPlus = q.SFPlus,
        //                          Price = q.Price,
        //                          TotPricePSF = q.TotPricePSF,
        //                          SlabPrice = q.SlabPrice,
        //                          Subtot = q.Subtot,
        //                          FabMin = q.FabMin,
        //                          FabMinPrice = q.FabMinPrice,
        //                          FabPrice = q.FabPrice,
        //                          FabPricePrintOveride = q.FabPricePrintOveride,
        //                          FabPricePrintRounded = q.FabPricePrintRounded,
        //                          FabPricePrint = q.FabPricePrint,
        //                          TotSlabs = q.TotSlabs,
        //                          Total = q.Total,
        //                          SubtotFab = q.SubtotFab,
        //                          SubtotFabPrint = q.SubtotFabPrint,
        //                          ExtAmt = q.ExtAmt
        //                      }).ToArray();

        //    if (quoteStone == null)
        //    {
        //        return NotFound();
        //    }

        //    return Ok(quoteStone);
        //}

        public object GetHoldInventory(int QuoteId)
        {
            var inventory = from s in db.StoneInventorys
                            where s.Inactive == false && s.QuoteId == QuoteId
                            join t in db.Stones
                            on s.StoneId equals t.Id into st
                            from t in st.DefaultIfEmpty()
                            join v in db.Vendors
                            on s.VendorId equals v.Id into sv
                            from v in sv.DefaultIfEmpty()
                            join e in db.Edges 
                            on s.EdgeId equals e.Id into se
                            from e in se.DefaultIfEmpty()
                            select new
                            {
                                Id = s.Id,
                                StoneId = s.StoneId,
                                StoneName = t.StoneName + " " + t.Thickness,
                                SerialNo = s.SerialNo,
                                Length = s.Length,
                                Width = s.Width,
                                SF = Math.Round((double)(s.Length * s.Width) / 144.00, 2),
                                UnitPrice = s.UnitPrice,
                                SalesTax = s.SalesTax,
                                Total = (s.UnitPrice + s.UnitPrice * ((s.SalesTax / 100) ?? 0)) * ((double)(s.Length * s.Width) / 144.00),
                                LotNo = s.LotNo,
                                VendorName = v.VendorName,
                                Consignment = s.Consignment,
                                DateSold = s.DateSold,
                                QuoteId = s.QuoteId,
                                EdgeId = s.EdgeId,
                                EdgeName = e.EdgeName,
                                Inactive = s.Inactive
                            };


            return inventory;
        }


        [ResponseType(typeof(QuoteStone))]
        public async Task<IHttpActionResult> GetQuoteStone(int id)
        {
            QuoteStone quoteStone = await db.QuoteStones.FindAsync(id);
            if (quoteStone == null)
            {
                return NotFound();
            }

            return Ok(quoteStone);
        }

        // PUT: api/Stone/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutQuoteStone(int id, QuoteStone quoteStone)
        {
            if (!ModelState.IsValid)
            {
                Console.WriteLine("problem direng dapita...");
                return BadRequest(ModelState);
            }

            if (id != quoteStone.Id)
            {
                Console.WriteLine("dire pud porbida..");
                return BadRequest();
            }

            //db.Entry(quoteStone).State = EntityState.Modified;
            var qsInDB = db.QuoteStones.Single(d => d.Id == quoteStone.Id);
            var entry = db.Entry(qsInDB);
            entry.CurrentValues.SetValues(quoteStone);

            var origStoneId = Convert.ToInt32(entry.OriginalValues["StoneId"]);
            var curStoneId = Convert.ToInt32(entry.CurrentValues["StoneId"]);

            if(origStoneId != curStoneId)
            {
                var inventory = db.StoneInventorys.Where(o => o.QuoteId == quoteStone.QuoteId && o.StoneId == origStoneId);
                foreach(var inv in inventory)
                {
                    inv.QuoteId = null;
                }
            }


            try
            {
                await db.SaveChangesAsync();
                var stoneData = from q in db.QuoteStones
                                where q.Id == quoteStone.Id 
                                join t in db.Stones
                                on q.StoneId equals t.Id
                                select new
                                {
                                    Id = q.Id,
                                    StoneId = q.StoneId,
                                    EdgeId = q.EdgeId,
                                    StoneName = t.StoneName + " " + t.Thickness,
                                    PublishedPrice = q.PublishedPrice,
                                    Length = q.Length,
                                    Width = q.Width,
                                    SquareFeet = q.SquareFeet,
                                    StateTax = q.StateTax,
                                    Surcharge = q.Surcharge,
                                    NSlab = q.NSlab,
                                    SquareFeetQty = q.SquareFeetQty,
                                    NSlabSFdiv = q.NSlabSFdiv,
                                    NSlabSF = q.NSlabSF,
                                    SF = q.SF,
                                    PlusVal = q.PlusVal,
                                    SFPlus = q.SFPlus,
                                    Price = q.Price,
                                    TotPricePSF = q.TotPricePSF,
                                    SlabPrice = q.SlabPrice,
                                    Subtot = q.Subtot,
                                    FabMin = q.FabMin,
                                    FabMinPrice = q.FabMinPrice,
                                    FabPrice = q.FabPrice,
                                    FabPricePrintOveride = q.FabPricePrintOveride,
                                    FabPricePrintRounded = q.FabPricePrintRounded,
                                    FabPricePrint = q.FabPricePrint,
                                    TotSlabs = q.TotSlabs,
                                    Total = q.Total,
                                    SubtotFab = q.SubtotFab,
                                    SubtotFabPrint = q.SubtotFabPrint,
                                    ExtAmt = q.ExtAmt
                                };

                return CreatedAtRoute("DefaultApi", new { id = quoteStone.Id }, stoneData.FirstOrDefault());

            }
            catch (DbUpdateConcurrencyException)
            {
                if (!QuoteStoneExists(id))
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

        // POST: api/Stone
        [ResponseType(typeof(QuoteStone))]
        public async Task<IHttpActionResult> PostQuoteStone(QuoteStone quoteStone)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.QuoteStones.Add(quoteStone);
            await db.SaveChangesAsync();

            //foreach (var qm in db.QuoteMeasures.Where(x => x.QuoteStoneId == null && x.QuoteId == quoteStone.QuoteId).ToList())
            //{
            //    qm.QuoteStoneId = quoteStone.Id;
            //}
            //await db.SaveChangesAsync();

            var stoneData = from q in db.QuoteStones
                            where q.Id == quoteStone.Id
                            join t in db.Stones
                            on q.StoneId equals t.Id
                            select new
                            {
                                Id = q.Id,
                                StoneId = q.StoneId,
                                StoneName = t.StoneName + " " + t.Thickness,
                                EdgeId = q.EdgeId,
                                PublishedPrice = q.PublishedPrice,
                                Length = q.Length,
                                Width = q.Width,
                                SquareFeet = q.SquareFeet,
                                StateTax = q.StateTax,
                                Surcharge = q.Surcharge,
                                NSlab = q.NSlab,
                                SquareFeetQty = q.SquareFeetQty,
                                NSlabSFdiv = q.NSlabSFdiv,
                                NSlabSF = q.NSlabSF,
                                SF = q.SF,
                                PlusVal = q.PlusVal,
                                SFPlus = q.SFPlus,
                                Price = q.Price,
                                TotPricePSF = q.TotPricePSF,
                                SlabPrice = q.SlabPrice,
                                Subtot = q.Subtot,
                                FabMin = q.FabMin,
                                FabMinPrice = q.FabMinPrice,
                                FabPrice = q.FabPrice,
                                FabPricePrintOveride = q.FabPricePrintOveride,
                                FabPricePrintRounded = q.FabPricePrintRounded,
                                FabPricePrint = q.FabPricePrint,
                                TotSlabs = q.TotSlabs,
                                Total = q.Total,
                                SubtotFab = q.SubtotFab,
                                SubtotFabPrint = q.SubtotFabPrint,
                                ExtAmt = q.ExtAmt
                            };

            return CreatedAtRoute("DefaultApi", new { id = quoteStone.Id }, stoneData.FirstOrDefault());
        }

        // DELETE: api/Stone/5
        [ResponseType(typeof(QuoteStone))]
        public async Task<IHttpActionResult> DeleteQuoteStone(int id)
        {
            QuoteStone quoteStone = await db.QuoteStones.FindAsync(id);
            if (quoteStone == null)
            {
                return NotFound();
            }

            db.QuoteStones.Remove(quoteStone);
            await db.SaveChangesAsync();

            return Ok(quoteStone);
        }

        public async Task<IHttpActionResult> PutInventory(int Id, int? QuoteId, int EdgeId)
        {
            //var inventory = db.StoneInventorys.Where(o => o.Id == Id).Single();
            var inventory = db.StoneInventorys.Find(Id);

            try
            {
                if (inventory.QuoteId == null)
                {
                    inventory.QuoteId = QuoteId;
                    inventory.EdgeId = EdgeId;
                }
                else
                {
                    inventory.QuoteId = null;
                    inventory.EdgeId = null;
                }

                await db.SaveChangesAsync();
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

        private bool QuoteStoneExists(int id)
        {
            return db.QuoteStones.Count(e => e.Id == id) > 0;
        }
    }
}