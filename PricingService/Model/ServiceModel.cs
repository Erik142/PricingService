using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PricingService.Model
{
    public class ServiceModel
    {
        [Required]
        public int ConsumerId { get; set; }
        public string ServiceName { get; set; }
    }
}
