using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PricingService.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PricingService.Database;
using PricingService.Calculations;
using Microsoft.EntityFrameworkCore;

namespace PricingService.Controllers
{
    [Produces("application/json")]
    [ApiController]
    [Route("/api/v1/[controller]")]
    public class PriceController : ControllerBase
    {
        private ILogger<PriceController> _logger;
        private PricingDbContext _dbContext;

        public PriceController(ILogger<PriceController> logger, PricingDbContext dbContext)
        {
            this._logger = logger;
            this._dbContext = dbContext;
        }
        /// <summary>
        /// Retrieves the total price for all used services for the specified customer within the specified time period.
        /// If "EndDate" is not specified, the end date will be the date of today.
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="model"></param>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET api/v1/price/50
        ///     {
        ///         "StartDate": "2010-03-29",
        ///         "EndDate": "2020-06-05"
        ///     }
        /// </remarks>
        [Consumes("application/json")]
        [HttpGet("{customerId:int}")]
        public IActionResult GetTotal(int customerId, PriceFilterModel model)
        {
            CustomerModel customer = _dbContext.Customers
                .Include(x => x.FreeDays)
                .Include(x => x.UsedServices)
                .ThenInclude(x => x.BillingDays)
                .Include(x => x.UsedServices)
                .ThenInclude(x => x.Discount)
                .FirstOrDefault(x => x.Id == customerId);

            if (customer == null)
            {
                return new JsonResult(new
                {
                    Error = $"The customer with the id {customerId} does not exist."
                });
            }

            DateTime endDate = DateTime.Now;

            if (model.EndDate != null)
            {
                endDate = (DateTime)model.EndDate;
            }

            try
            {
                return new JsonResult(new
                {
                    Price = PriceCalculator.GetPrice(customer, model.StartDate, endDate)
                });
            }
            catch (Exception e)
            {
                return new JsonResult(new
                {
                    Error = e.Message
                });
            }
        }
    }
}
