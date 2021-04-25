using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PricingService.Model
{
    public class CustomerModel
    {
        [Key]
        public int Id { get; set; }
        public FreeDaysModel FreeDays { get; set; }
        public List<ServiceModel> UsedServices { get; set; }

    }
}
