using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PricingService.Model
{
    public class DiscountModel : ServiceModel
    {
        [Required]
        public double DiscountPercent { get; set; }
        public DateTime EndDate { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
    }
}
