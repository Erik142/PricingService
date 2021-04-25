using Microsoft.EntityFrameworkCore;
using PricingService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PricingService.Database
{
    public class DatabaseSeeder
    {
        public void Seed(PricingDbContext dbContext)
        {
            DayOfWeekModel[] billingDaysA = new[]
            {
                new DayOfWeekModel()
                {
                    Day = DayOfWeek.Monday
                },
                new DayOfWeekModel()
                {
                    Day = DayOfWeek.Tuesday
                },
                new DayOfWeekModel()
                {
                    Day = DayOfWeek.Wednesday
                },
                new DayOfWeekModel()
                {
                    Day = DayOfWeek.Thursday
                },
                new DayOfWeekModel()
                {
                    Day = DayOfWeek.Friday
                },
            };

            DayOfWeekModel[] billingDaysB = new[]
            {
                new DayOfWeekModel()
                {
                    Day = DayOfWeek.Monday
                },
                new DayOfWeekModel()
                {
                    Day = DayOfWeek.Tuesday
                },
                new DayOfWeekModel()
                {
                    Day = DayOfWeek.Wednesday
                },
                new DayOfWeekModel()
                {
                    Day = DayOfWeek.Thursday
                },
                new DayOfWeekModel()
                {
                    Day = DayOfWeek.Friday
                },
            };

            ServiceDefinitionModel[] serviceDefinitions = new[] {
                new ServiceDefinitionModel()
                {
                    ServiceName = "ServiceA",
                    Price = 0.2,
                    BillingDays = new List<DayOfWeekModel>(billingDaysA)
                },
                new ServiceDefinitionModel()
                {
                    ServiceName = "ServiceB",
                    Price = 0.24,
                    BillingDays = new List<DayOfWeekModel>(billingDaysB)
                },
                new ServiceDefinitionModel()
                {
                    ServiceName = "ServiceC",
                    Price = 0.4
                }
            };

            ServiceDefinitionModel[] oldServiceDefinitions = dbContext.ServiceDefinitions.Include(x => x.BillingDays).ToArray().Where(x => x.GetType() == typeof(ServiceDefinitionModel)).ToArray();
            DayOfWeekModel[] oldDays = oldServiceDefinitions.SelectMany(x => x.BillingDays).ToArray();
            dbContext.Days.RemoveRange(oldDays);
            dbContext.ServiceDefinitions.RemoveRange(oldServiceDefinitions);
            dbContext.ServiceDefinitions.AddRange(serviceDefinitions);

            dbContext.SaveChanges();
        }
    }
}
