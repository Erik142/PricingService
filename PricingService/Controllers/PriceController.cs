using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PricingService.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace PricingService.Controllers
{
    [Produces("application/json")]
    [ApiController]
    [Route("/api/v1/[controller]")]
    public class PriceController : Controller
    {
        private ILogger<PriceController> _logger;

        public PriceController(ILogger<PriceController> logger)
        {
            this._logger = logger;
        }

        [Consumes("application/json")]
        [HttpGet]
        public IActionResult GetTotal(PriceModel model)
        {
            throw new NotImplementedException();
        }
    }
}
