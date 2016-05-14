using ProStoneIMS2015.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ProStoneIMS2015.ControllersApi
{
    public class TotqsController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public object GetTotqs(int id)
        {
            return db.QuoteServices.Where(e => e.QuoteId == id).Sum(x => x.Amount);
        }
    }
}
