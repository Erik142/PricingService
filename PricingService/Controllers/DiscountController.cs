using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PricingService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PricingService.Controllers
{
    [Produces("application/json")]
    [ApiController]
    [Route("api/v1/[controller]")]
    public class DiscountController : Controller
    {
        private ILogger<DiscountController> _logger;

        public DiscountController(ILogger<DiscountController> logger)
        {
            this._logger = logger;
        }

        [Consumes("application/json")]
        [HttpGet]
        public IActionResult GetDiscount(ServiceModel model)
        {
            throw new NotImplementedException();
        }

        [Consumes("application/json")]
        [HttpPost]
        [HttpPut]
        public IActionResult AddDiscount(DiscountModel model)
        {
            throw new NotImplementedException();
        }

        [Consumes("application/json")]
        [HttpDelete]
        public IActionResult DeleteDiscount(ServiceModel model)
        {
            throw new NotImplementedException();
        }

        [Consumes("application/json")]
        [HttpPost("baseprice")]
        [HttpPut("baseprice")]
        public IActionResult SetBasePrice(BasePriceModel model)
        {
            throw new NotImplementedException();
        }

        [Consumes("application/json")]
        [HttpGet("baseprice")]
        public IActionResult GetBasePrice(ServiceModel model)
        {
            throw new NotImplementedException();
        }

        [Consumes("application/json")]
        [HttpPost("freedays")]
        [HttpPut("freedays")]
        public IActionResult SetFreeDays(FreeDaysModel model)
        {
            throw new NotImplementedException();
        }

        [Consumes("application/json")]
        [HttpGet("freedays")]
        public IActionResult GetFreeDays(ServiceModel model)
        {
            throw new NotImplementedException();
        }
    }
}
