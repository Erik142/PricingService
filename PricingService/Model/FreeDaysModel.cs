using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PricingService.Model
{
    public class FreeDaysModel : ServiceModel
    {
        [Key]
        public int Id { get; set; }
        public ulong FreeDays { get; set; }
    }
}
