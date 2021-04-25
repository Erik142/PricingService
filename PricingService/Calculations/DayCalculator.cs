using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PricingService.Calculations
{
    public static class DayCalculator
    {
        public static int GetDays(DateTime from, DateTime to, DayOfWeek[] days = null)
        {
            int daysBetween = (int)(to - from).TotalDays;

            if (days != null && days.Length > 0)
            {
                daysBetween = Enumerable.Range(0, daysBetween).Count(x => from.AddDays(x).DayOfWeek.In(days));
            }

            return daysBetween;
        }

        private static bool In(this DayOfWeek source, DayOfWeek[] validDays)
        {
            return validDays.Contains(source);
        }
    }
}
