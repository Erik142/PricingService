using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PricingService.Model
{
    public class DiscountModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string ServiceName { get; set; }
        [Required]
        public double DiscountPercent { get; set; }
        public DateTime? EndDate { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
    }
}
