using Microsoft.AspNet.Identity.EntityFramework;
using Multitenant.Interception.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace ProStoneIMS2015.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext() : base("ProStoneIMSConn", throwIfV1Schema: false)
        {
            Configuration.LazyLoadingEnabled = false;
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        //Classes affected by Migrations
        public DbSet<Subscriber> Subscribers { get; set; }
        public DbSet<Quote> Quotes { get; set; }
        public DbSet<Phone> Phones { get; set; }
        public DbSet<Stone> Stones { get; set; }
        public DbSet<StoneInventory> StoneInventorys { get; set; }
        public DbSet<SinkInventory> SinkInventorys { get; set; }
        public DbSet<Stone_type> Stone_types { get; set; }
        public DbSet<Edge> Edges { get; set; }
        public DbSet<Sink> Sinks { get; set; }
        public DbSet<Sink_type> Sink_types { get; set; }
        public DbSet<Jservice> Jservices { get; set; }
        public DbSet<Jservice_type> Jservice_types { get; set; }
        public DbSet<Measurement> Measurements { get; set; }
        public DbSet<Vendor> Vendors { get; set; }
        public DbSet<Lead> Leads { get; set; }
        public DbSet<Assignedto> Assignedtos { get; set; }
        public DbSet<Salesman> Salesmans { get; set; }
        public DbSet<PayStatus> PayStatuss { get; set; }
        public DbSet<Backsplash> Backsplashs { get; set; }
        public DbSet<TaskStatus> TaskStatuss { get; set; }
        public DbSet<QuoteHistory> QuoteHistorys { get; set; }
        public DbSet<QuoteSink> QuoteSinks { get; set; }
        public DbSet<QuoteService> QuoteServices { get; set; }

        public System.Data.Entity.DbSet<ProStoneIMS2015.Models.QuoteStone> QuoteStones { get; set; }

        public System.Data.Entity.DbSet<ProStoneIMS2015.Models.QuoteMeasure> QuoteMeasures { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Stone>().HasRequired(pc => pc.subscriber)
                .WithMany().HasForeignKey(pc => pc.TenantId).WillCascadeOnDelete(false);

            modelBuilder.Entity<Edge>().HasRequired(pc => pc.subscriber)
                .WithMany().HasForeignKey(pc => pc.TenantId).WillCascadeOnDelete(false);

            modelBuilder.Entity<Sink>().HasRequired(pc => pc.subscriber)
                .WithMany().HasForeignKey(pc => pc.TenantId).WillCascadeOnDelete(false);

            modelBuilder.Entity<Jservice>().HasRequired(pc => pc.subscriber)
                .WithMany().HasForeignKey(pc => pc.TenantId).WillCascadeOnDelete(false);

            modelBuilder.Entity<Measurement>().HasRequired(pc => pc.subscriber)
                .WithMany().HasForeignKey(pc => pc.TenantId).WillCascadeOnDelete(false);

            modelBuilder.Entity<Vendor>().HasRequired(pc => pc.subscriber)
                .WithMany().HasForeignKey(pc => pc.TenantId).WillCascadeOnDelete(false);

            modelBuilder.Entity<Quote>().HasRequired(pc => pc.subscriber)
                .WithMany().HasForeignKey(pc => pc.TenantId).WillCascadeOnDelete(false);


            modelBuilder.Entity<QuoteHistory>().HasRequired(pc => pc.subscriber)
                .WithMany().HasForeignKey(pc => pc.TenantId).WillCascadeOnDelete(false);
            modelBuilder.Entity<QuoteSink>().HasRequired(pc => pc.subscriber)
                .WithMany().HasForeignKey(pc => pc.TenantId).WillCascadeOnDelete(false);
            modelBuilder.Entity<QuoteService>().HasRequired(pc => pc.subscriber)
                .WithMany().HasForeignKey(pc => pc.TenantId).WillCascadeOnDelete(false);
            modelBuilder.Entity<QuoteStone>().HasRequired(pc => pc.subscriber)
                .WithMany().HasForeignKey(pc => pc.TenantId).WillCascadeOnDelete(false);

            //modelBuilder.Entity<QuoteMeasure>().HasRequired(pc => pc.Quote)
            //    .WithMany().HasForeignKey(pc => pc.QuoteId).WillCascadeOnDelete(true);

            //modelBuilder.Entity<QuoteHistory>().HasRequired(pc => pc.Quote)
            //    .WithMany().HasForeignKey(pc => pc.Id).WillCascadeOnDelete(true);
            //modelBuilder.Entity<QuoteSink>().HasRequired(pc => pc.Quote)
            //    .WithMany().HasForeignKey(pc => pc.Id).WillCascadeOnDelete(true);
            //modelBuilder.Entity<QuoteService>().HasRequired(pc => pc.Quote)
            //    .WithMany().HasForeignKey(pc => pc.Id).WillCascadeOnDelete(true);
            //modelBuilder.Entity<QuoteStone>().HasRequired(pc => pc.Quote)
            //    .WithMany().HasForeignKey(pc => pc.Id).WillCascadeOnDelete(true);
            //modelBuilder.Entity<QuoteMeasure>().HasRequired(pc => pc.QuoteStones)
            //    .WithMany().HasForeignKey(pc => pc.QuoteStones).WillCascadeOnDelete(true);


            var conv = new AttributeToTableAnnotationConvention<TenantAwareAttribute, string>
                    (TenantAwareAttribute.TenantAnnotation, (type, attributes) => attributes.Single().ColumnName);

            modelBuilder.Conventions.Add(conv);
        }

        public System.Data.Entity.DbSet<ProStoneIMS2015.Models.QuotePayment> QuotePayments { get; set; }

    }

}