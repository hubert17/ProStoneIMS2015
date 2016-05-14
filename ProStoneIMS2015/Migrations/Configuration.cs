using Microsoft.AspNet.Identity;

namespace ProStoneIMS2015.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<ProStoneIMS2015.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(ProStoneIMS2015.Models.ApplicationDbContext context)
        {
            var subscriber0 = new Subscriber()
            {
                CompanyName = "_ProStoneIMS_Admin_Subscriber",
                Email = "admin@prostoneims.com",
                SubscriberKey = "psims0",
                FirstName = "_ProStoneIMS",
                LastName = "_AdminSubscriber",
                StateTax = 8.25,
                PlusVal = 15,
                FabMin = 25,
                FabMinPrice = 1.65
            };

            var subscriber1 = new Subscriber()
            {
                CompanyName = "DFW Wholesale Granite",
                Email = "dhitt0327@gmail.com",
                SubscriberKey = "dfw327",
                FirstName = "David",
                LastName = "Hitt",
                StateTax = 8.25,
                PlusVal = 15,
                FabMin = 25,
                FabMinPrice = 1.65
            };

            var subscriber2 = new Subscriber()
            {
                CompanyName = "ProStone Philippines",
                Email = "hubert17@facebook.com",
                SubscriberKey = "psp117",
                FirstName = "Bernard",
                LastName = "Gabon"
            };

            context.Subscribers.AddOrUpdate(
              p => p.SubscriberKey, subscriber0, subscriber1, subscriber2
            );

            var Rstore = new RoleStore<IdentityRole>(context);
            var Rmanager = new RoleManager<IdentityRole>(Rstore);
            if (!context.Roles.Any(r => r.Name == "admin"))
            {
                Rmanager.Create(new IdentityRole { Name = "admin" });
            }
            if (!context.Roles.Any(r => r.Name == "subscriber"))
            {
                Rmanager.Create(new IdentityRole { Name = "subscriber" });
            }
            if (!context.Roles.Any(r => r.Name == "user"))
            {
                Rmanager.Create(new IdentityRole { Name = "user" });
            }

            ApplicationUser user = new ApplicationUser();
            var store = new UserStore<ApplicationUser>(context);
            var manager = new UserManager<ApplicationUser>(store);

            //create admin
            if (!context.Users.Any(u => u.UserName == "admin@prostoneims.com"))
            {
                user = new ApplicationUser
                {
                    UserName = "admin@prostoneims.com",
                    Email = "admin@prostoneims.com",
                    FirstName = "Admin",
                    LastName = "Bernard",
                    TenantId = context.Subscribers.Where(x => x.SubscriberKey == subscriber0.SubscriberKey).Select(s => s.TenantId).FirstOrDefault(),
                    EmailConfirmed = true
                };
                manager.Create(user, "Sds1234....");
                manager.AddToRoles(user.Id, new string[] { "admin" });
            }
            if (!context.Users.Any(u => u.UserName == "dhitt0327@prostoneims.com"))
            {
                user = new ApplicationUser
                {
                    UserName = "dhitt0327@prostoneims.com",
                    Email = "dhitt0327@prostoneims.com",
                    FirstName = "Admin",
                    LastName = "David",
                    TenantId = context.Subscribers.Where(x => x.SubscriberKey == subscriber0.SubscriberKey).Select(s => s.TenantId).FirstOrDefault(),
                    EmailConfirmed = true
                };
                manager.Create(user, "2*****..");
                manager.AddToRoles(user.Id, new string[] { "admin" });
            }
            //create user-subscriber
            if (!context.Users.Any(u => u.UserName == subscriber1.Email))
            {
                user = new ApplicationUser
                {
                    UserName = subscriber1.Email,
                    Email = subscriber1.Email,
                    FirstName = subscriber1.FirstName,
                    LastName = subscriber1.LastName,
                    TenantId = context.Subscribers.Where(x => x.SubscriberKey == subscriber1.SubscriberKey).Select(s => s.TenantId).FirstOrDefault(),
                    EmailConfirmed = true                    
                };
                manager.Create(user, "2******....");
                manager.AddToRoles(user.Id, new string[] { "subscriber", "user" });
            }
            if (!context.Users.Any(u => u.UserName == subscriber2.Email))
            {
                user = new ApplicationUser
                {
                    UserName = subscriber2.Email,
                    Email = subscriber2.Email,
                    FirstName = subscriber2.FirstName,
                    LastName = subscriber2.LastName,
                    TenantId = context.Subscribers.Where(x => x.SubscriberKey == subscriber2.SubscriberKey).Select(s => s.TenantId).FirstOrDefault(),
                    EmailConfirmed = true

                };
                manager.Create(user, "Sds1234....");
                manager.AddToRoles(user.Id, new string[] { "subscriber", "user" });
            }

            context.Edges.AddOrUpdate(
              p => p.EdgeName,
              new Edge { EdgeName = "1/4 Bevel", Price = 0.00, ImageFilename = "1fourthbevel.png", TenantId = subscriber1.TenantId },
              new Edge { EdgeName = "Flat Polish", Price = 0.00, ImageFilename = "flatpolish.png", TenantId = subscriber1.TenantId },
              new Edge { EdgeName = "Full Bull", Price = 18.00, ImageFilename = "bullnose_full.jpg", TenantId = subscriber1.TenantId }
            );

            var Kitchen_Sink = new Sink_type { Name = "Kitchen Sink", TenantId = subscriber1.TenantId };
            var Vanity_Sink = new Sink_type { Name = "Vanity Sink", TenantId = subscriber1.TenantId };
            context.Sink_types.AddOrUpdate(
              p => p.Name, Kitchen_Sink, Vanity_Sink      
            );

            context.Sinks.AddOrUpdate(
              p => p.SinkName,
              new Sink { CatalogID = "LS-68", SinkName = "60/40 Double Bowl Kitchen Sink", SinkMiniName = "60/40", Price = 279.00, Type= Kitchen_Sink.Id, TenantId = subscriber1.TenantId },
              new Sink { CatalogID = "LS-88", SinkName = "50/50 Double Bowl Kitchen Sink", SinkMiniName = "50/50 ", Price = 279.00, Type = Kitchen_Sink.Id, TenantId = subscriber1.TenantId },
              new Sink { CatalogID = "LS-C2", SinkName = "Single Bowl Ceramic Sink - Bisque", SinkMiniName = "Bisque Oval", Price = 75.00, Type = Kitchen_Sink.Id, TenantId = subscriber1.TenantId },
              new Sink { CatalogID = "LS-C6", SinkName = "Rectangle Porecelain Vanity Sink - White", SinkMiniName = "Rectangle White", Price = 125.00, Type = Vanity_Sink.Id, TenantId = subscriber1.TenantId },
              new Sink { CatalogID = "***", SinkName = "Owner Provide", SinkMiniName = "Rectangle White", Price = 0.00, Type = Kitchen_Sink.Id, TenantId = subscriber1.TenantId },
              new Sink { CatalogID = "LSU-C2", SinkName = "Utility Sink Medium", SinkMiniName = "Medium Utility", Price = 199.00, Type = Vanity_Sink.Id, TenantId = subscriber1.TenantId }
            );

            context.Stone_types.AddOrUpdate(
                p => p.Name,
                    new Stone_type { Name = "Granite", TenantId = subscriber1.TenantId },
                    new Stone_type { Name = "Marble", TenantId = subscriber1.TenantId },
                    new Stone_type { Name = "Quarts", TenantId = subscriber1.TenantId }
            );

            context.Jservice_types.AddOrUpdate(
                p => p.Code,
                new Jservice_type { Code = "0", Name = String.Empty },
                new Jservice_type { Code = "T", Name = "Quantity from Square Feet" },
                new Jservice_type { Code = "Q", Name = "Quantity Input Enabled" }
            );

            context.Jservices.AddOrUpdate(
              p => p.ServiceName,
              new Jservice { ServiceName = "[Tear Out Countertops]", Price = 3.00, WOPrice = 1.00, ServiceCode = "T", TenantId = subscriber1.TenantId },
              new Jservice { ServiceName = "Undermount Utility Sink Cut and Polish", Price = 175.00, WOPrice = 40.00, TenantId = subscriber1.TenantId },
              new Jservice { ServiceName = "Undermount Vanity Sink", Price = 75.00, WOPrice = 30.00, TenantId = subscriber1.TenantId },
              new Jservice { ServiceName = "Drop in Stove Top Cut Out", Price = 60.00, WOPrice = 20.00, TenantId = subscriber1.TenantId },
              new Jservice { ServiceName = "Undermount Bar Sink Cut Out", Price = 125.00, WOPrice = 30.00, TenantId = subscriber1.TenantId },
              new Jservice { ServiceName = "Discount", Price = -1.00, WOPrice = 0.00, ServiceCode = "Q", TenantId = subscriber1.TenantId },
              new Jservice { ServiceName = "Extra Edging", Price = 1.00, WOPrice = 0.25, TenantId = subscriber1.TenantId }
            );

            context.PayStatuss.AddOrUpdate(
                p => p.Code,
                    new PayStatus { Code = "D", Name="Deposit" },
                    new PayStatus { Code = "F", Name = "Final Payment" }
            );

            context.Backsplashs.AddOrUpdate(
                p => p.Name,
                    new Backsplash { Name = "Owner Provide Tile" },
                    new Backsplash { Name = "Standard 4\"" },
                    new Backsplash { Name = "Tile Backsplash" },
                    new Backsplash { Name = "Full Height (aprox. 18\")" },
                    new Backsplash { Name = "None" }
            );

            context.TaskStatuss.AddOrUpdate(
                p => p.Code,
                    new TaskStatus { Name = "Measure", Code = "MEA" },
                    new TaskStatus { Name = "Qoute", Code = "QUO" },
                    new TaskStatus { Name = "Select Stone", Code = "SEL" },
                    new TaskStatus { Name = "Template", Code = "TEM" },
                    new TaskStatus { Name = "Fabrication", Code = "FAB" },
                    new TaskStatus { Name = "Install", Code = "INS" },
                    new TaskStatus { Name = "Completed", Code = "COM" },
                    new TaskStatus { Name = "Reference", Code = "REF" },
                    new TaskStatus { Name = "Repair", Code = "REP" },
                    new TaskStatus { Name = "Cabinets", Code = "CAB" },
                    new TaskStatus { Name = "Inactive", Code = "INA" }
            );

            context.Leads.AddOrUpdate(
                p => p.LeadName,
                    new Lead { LeadName = "Website", TenantId = subscriber1.TenantId },
                    new Lead { LeadName = "Sign", TenantId = subscriber1.TenantId },
                    new Lead { LeadName = "Referral", TenantId = subscriber1.TenantId },
                    new Lead { LeadName = "IMC", TenantId = subscriber1.TenantId },
                    new Lead { LeadName = "Daltile", TenantId = subscriber1.TenantId },
                    new Lead { LeadName = "Angie's List", TenantId = subscriber1.TenantId },
                    new Lead { LeadName = "After Sales Sign", TenantId = subscriber1.TenantId }
            );

            context.Salesmans.AddOrUpdate(
                p => p.SalesmanName,
                    new Salesman { SalesmanName = "David", TenantId = subscriber1.TenantId },
                    new Salesman { SalesmanName = "Reed", TenantId = subscriber1.TenantId },
                    new Salesman { SalesmanName = "Javiar", TenantId = subscriber1.TenantId },
                    new Salesman { SalesmanName = "Brandon", TenantId = subscriber1.TenantId }
            );

            context.Assignedtos.AddOrUpdate(
                p => p.AssignedtoName,
                    new Assignedto { AssignedtoName= "David", TenantId = subscriber1.TenantId },
                    new Assignedto { AssignedtoName = "Reed", TenantId = subscriber1.TenantId },
                    new Assignedto { AssignedtoName = "Javiar", TenantId = subscriber1.TenantId },
                    new Assignedto { AssignedtoName = "Brandon", TenantId = subscriber1.TenantId }
            );

            context.Measurements.AddOrUpdate(
                p => p.MeasureName,
                    new Measurement { MeasureName = "Counter", TenantId = subscriber1.TenantId },
                    new Measurement { MeasureName = "Upper Bar", TenantId = subscriber1.TenantId },
                    new Measurement { MeasureName = "Island", TenantId = subscriber1.TenantId },
                    new Measurement { MeasureName = "Vanity 1", TenantId = subscriber1.TenantId },
                    new Measurement { MeasureName = "Backsplash 4", TenantId = subscriber1.TenantId }
            );

            var quote1 = new Quote
            {
                Id = 1,
                LastName = "Gabon",
                FirstName = "Bernard",
                Address = "Douglas Macarthur st.",
                City = "Valencia",
                State = "Philippines",
                Zip = "8709",
                Phone = "09999999",
                Email = "hewbertgabon@gmail.com",

                JobNo = null,
                Verified = false,
                PayStatus = "D",
                Backsplash = 1,
                Lead = 1,
                DateCreated = DateTime.Now.Date,
                Salesman = 1,
                TaskDate = DateTime.Now.Date.AddDays(1),
                TaskTime = "10 am",
                TaskStatus = "INS",
                AssignedTo = 1,

                TenantId = subscriber1.TenantId
            };

            var quote2 = new Quote
            {
                FirstName = "David",
                LastName = "Hitt",
                Address = "Saginaw Village",
                City = "Fort Worth",
                State = "TX",
                Zip = "77212",
                Phone = "880000880",
                Email = "dhitt0327@gmail.com",
                JobNo = null,
                Verified = false,
                PayStatus = "D",
                Backsplash = 1,
                Lead = 1,
                DateCreated = DateTime.Now.Date,
                Salesman = 1,
                TaskDate = DateTime.Now.Date.AddDays(1),
                TaskTime = "10 am",
                TaskStatus = "INS",
                AssignedTo = 1,
                TenantId = subscriber1.TenantId
            };

            context.Quotes.AddOrUpdate( //p => p.LastName,  
                quote1, quote2
            );

            context.QuoteHistorys.AddOrUpdate(
                p => p.Id,
                    new QuoteHistory { TaskStatus = "INS", TaskDate = DateTime.Now.Date, TaskTime = "10:00 am", QuoteId = quote1.Id, TenantId = subscriber1.TenantId  },
                    new QuoteHistory { TaskStatus = "TEM", TaskDate = DateTime.Now.Date, TaskTime = "09:00 am", QuoteId = quote2.Id, TenantId = subscriber1.TenantId }
            );

            var vendor1 = new Vendor { VendorName = "Daltile", TenantId = subscriber1.TenantId };
            var vendor2 = new Vendor { VendorName = "IMC Stone", TenantId = subscriber1.TenantId };
            var vendor3 = new Vendor { VendorName = "Levantina", TenantId = subscriber1.TenantId };
            context.Vendors.AddOrUpdate(
                p => p.VendorName, vendor1, vendor2, vendor3
            );

            var stone1 = new Stone { StoneName = "Bianco Romano 3cm", TenantId = subscriber1.TenantId };
            var stone2 = new Stone { StoneName = "Alaska White Stock 3cm", TenantId = subscriber1.TenantId };
            var stone3 = new Stone { StoneName = "Ashen White 2cm", TenantId = subscriber1.TenantId };
            var stone4 = new Stone { StoneName = "Giallo Ornamental Stock 3cm", TenantId = subscriber1.TenantId };
            context.Stones.AddOrUpdate(
                p => p.StoneName,
                stone1, stone2, stone3, stone4,
                    new Stone { StoneName = "New Venetian Gold 3cm", TenantId = subscriber1.TenantId },
                    new Stone { StoneName = "Santa Cecilia 3cm", TenantId = subscriber1.TenantId },
                    new Stone { StoneName = "Rainforest Green 2cm", TenantId = subscriber1.TenantId }
            );

            //context.StoneInventorys.AddOrUpdate(
            //    p => p.SerialNo,
            //        new StoneInventory { SerialNo = "serial1", Length = 121, Width = 246, LotNo = "LOT1", VendorId = vendor1.Id, StoneId = stone1.Id, TenantId = subscriber1.TenantId },
            //        new StoneInventory { SerialNo = "serial2", Length = 122, Width = 247, LotNo = "LOT2", VendorId = vendor2.Id, StoneId = stone1.Id, TenantId = subscriber1.TenantId },
            //        new StoneInventory { SerialNo = "serial3", Length = 123, Width = 248, LotNo = "LOT3", VendorId = vendor2.Id, StoneId = stone2.Id, TenantId = subscriber1.TenantId, QuoteId = quote2.Id },
            //        new StoneInventory { SerialNo = "serial4", Length = 124, Width = 249, LotNo = "LOT4", VendorId = vendor3.Id, StoneId = stone3.Id, TenantId = subscriber1.TenantId },
            //        new StoneInventory { SerialNo = "serial5", Length = 125, Width = 240, LotNo = "LOT5", VendorId = vendor1.Id, StoneId = stone4.Id, TenantId = subscriber1.TenantId }
            //);

        }
    }
}
