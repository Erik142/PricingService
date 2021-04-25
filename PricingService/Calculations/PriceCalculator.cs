using PricingService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PricingService.Calculations
{
    public static class PriceCalculator
    {
        public static double GetPrice(CustomerModel customer, DateTime startDate, DateTime endDate)
        {
            double finalPrice = 0;
            int freeDays = customer.FreeDays != null ? customer.FreeDays.FreeDays : 0;

            if (((endDate - startDate).TotalDays - freeDays) <= 0)
            {
                if ((endDate - startDate).TotalDays <= 0)
                {
                    throw new Exception("Cannot calculate service price. The end date was evaluated to occur before the start date.");
                }

                return 0;
            }

            foreach (ServiceModel service in customer.UsedServices)
            {
                DateTime from = service.StartDate != null && DateTime.Compare(startDate, (DateTime)service.StartDate) > 0 ? startDate : (DateTime)service.StartDate;
                DateTime to = service.EndDate != null && DateTime.Compare(endDate, (DateTime)service.EndDate) > 0 ? (DateTime)service.EndDate : endDate;

                if (DateTime.Compare(to, from) < 0)
                {
                    throw new Exception("Cannot calculate service price. The end date was evaluated to occur before the start date.");
                }

                int daysUsed = DayCalculator.GetDays(from, to, service.BillingDays.Select(x => x.Day).ToArray()) - freeDays;

                if (daysUsed <= 0)
                {
                    freeDays -= daysUsed;
                    continue;
                }
                else if (freeDays > 0)
                {
                    freeDays = 0;
                }

                finalPrice += GetServicePrice(daysUsed, service);
            }

            return finalPrice;
        }

        private static double GetServicePrice(int usedDays, ServiceModel service)
        {
            double discountFactor = service.Discount != null ? service.Discount.DiscountPercent / 100.0 : 0;

            return service.Price * usedDays * (1 - discountFactor);
        }
    }
}
