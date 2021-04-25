using Microsoft.EntityFrameworkCore;
using PricingService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PricingService.Database
{
    public class PricingDbContext : DbContext
    {
        public DbSet<CustomerModel> Customers { get; set; }
        public DbSet<ServiceModel> Services { get; set; }
        public DbSet<DiscountModel> Discounts { get; set; }
        public DbSet<FreeDaysModel> FreeDays { get; set; }
        public DbSet<ServiceDefinitionModel> ServiceDefinitions { get; set; }
        public DbSet<DayOfWeekModel> Days { get; set; }

        public PricingDbContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<DayOfWeekModel>()
                .Property(x => x.Day)
                .HasConversion<int>();
        }
    }
}
