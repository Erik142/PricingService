using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PricingService.Model
{
    public class BasePriceModel : ServiceModel
    {
        [Required]
        public double BasePrice { get; set; }
    }
}
