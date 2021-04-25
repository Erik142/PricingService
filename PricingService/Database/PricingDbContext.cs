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
        public DbSet<BasePriceModel> BasePrices { get; set; }
        public DbSet<DiscountModel> Discounts { get; set; }
        public DbSet<FreeDaysModel> FreeDays { get; set; }
    }
}
