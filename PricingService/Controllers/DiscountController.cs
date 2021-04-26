using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PricingService.Database;
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
    public class DiscountController : ControllerBase
    {
        private ILogger<DiscountController> _logger;
        private PricingDbContext _dbContext;

        public DiscountController(ILogger<DiscountController> logger, PricingDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        /// <summary>
        /// Retrieves the discount in percent for the specified service and customer.
        /// </summary>
        /// <param name="customerId">The customer id</param>
        /// <param name="serviceName">The service name</param>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET api/v1/discount/50/serviceb
        /// </remarks>
        [Consumes("application/json")]
        [HttpGet("{customerId:int}/{serviceName}")]
        public IActionResult GetDiscount(int customerId, string serviceName)
        {
            CustomerModel customer = _dbContext.Customers
                .Include(x => x.FreeDays)
                .Include(x => x.UsedServices)
                .ThenInclude(x => x.Discount)
                .FirstOrDefault(x => x.Id == customerId);

            if (customer == null)
            {
                ModelState.AddModelError("customerId", "The customer with the specified customer id does not exist.");
                return BadRequest(ModelState);
            }

            if (!customer.UsedServices.Any(x => x.ServiceName.ToUpperInvariant() == serviceName.ToUpperInvariant()))
            {
                ModelState.AddModelError("serviceName", "The customer does not use the service with the specified name.");
                return BadRequest(ModelState);
            }

            DiscountModel discountModel = customer.UsedServices.First(x => x.ServiceName.ToUpperInvariant() == serviceName.ToUpperInvariant()).Discount;

            double discount = discountModel != null ? discountModel.DiscountPercent : 0;

            return new JsonResult(new
            {
                DiscountPercent = discount
            });
        }

        /// <summary>
        /// Adds or updates the discount for a specific service and customer.
        /// </summary>
        /// <param name="customerId">The customer id</param>
        /// <param name="model">The request body, parsed as a DiscountModel object</param>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST/PUT api/v1/discount/50
        ///     {
        ///         "ServiceName": "ServiceA",
        ///         "StartDate": "2010-02-01",
        ///         "EndDate": "2015-12-24",
        ///         "DiscountPercent": 50
        ///     }
        /// </remarks>
        [Consumes("application/json")]
        [HttpPost("{customerId:int}")]
        [HttpPut("{customerId:int}")]
        public async Task<IActionResult> AddDiscount(int customerId, DiscountModel model)
        {
            CustomerModel customer = _dbContext.Customers
                .Include(x => x.FreeDays)
                .Include(x => x.UsedServices)
                .ThenInclude(x => x.Discount)
                .FirstOrDefault(x => x.Id == customerId);

            if (customer == null)
            {
                ModelState.AddModelError("customerId", "The customer with the specified customer id does not exist.");
                return BadRequest(ModelState);
            }

            if (!customer.UsedServices.Any(x => x.ServiceName == model.ServiceName))
            {
                ModelState.AddModelError("ServiceName", "The customer does not use the service with the specified name.");
                return BadRequest(ModelState);
            }

            ServiceModel service = customer.UsedServices.First(x => x.ServiceName == model.ServiceName);

            if (Request.Method == "POST")
            {
                if (service.Discount != null)
                {
                    ModelState.AddModelError("ServiceName", "The specified service already has a discount. Use the PUT method to update the discount value.");
                    return BadRequest(ModelState);
                }

                service.Discount = model;
            }
            else
            {
                if (service.Discount == null)
                {
                    ModelState.AddModelError("ServiceName", "The specified service does not have any discount. Use the POST method to add a new discount.");
                    return BadRequest(ModelState);
                }

                service.Discount.DiscountPercent = model.DiscountPercent;
                service.Discount.StartDate = model.StartDate;
                service.Discount.EndDate = model.EndDate;
            }

            await _dbContext.SaveChangesAsync();
            return Ok();
        }
    }
}
