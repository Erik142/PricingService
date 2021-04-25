using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PricingService.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PricingService.Extensions
{
    public static class IHostExtensions
    {
        public static IHost SeedData(this IHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var dbContext = services.GetService<PricingDbContext>();

                DatabaseSeeder seeder = new DatabaseSeeder();
                seeder.Seed(dbContext);
            }

            return host;
        }
    }
}
