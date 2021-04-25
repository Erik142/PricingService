using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PricingService.Model
{
    public class ServiceModel : ServiceDefinitionModel
    {
        public int CustomerId { get; set; }
        [Required]
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DiscountModel Discount { get; set; }
    }
}
