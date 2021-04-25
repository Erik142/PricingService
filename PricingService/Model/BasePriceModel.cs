using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PricingService.Model
{
    public class BasePriceModel : ServiceModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public double BasePrice { get; set; }
    }
}
