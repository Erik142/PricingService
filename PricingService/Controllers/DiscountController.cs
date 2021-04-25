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
    public class DiscountController : Controller
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

        /// <summary>
        /// Deletes the discount for the specified service and customer.
        /// </summary>
        /// <param name="customerId">The customer id</param>
        /// <param name="serviceName">The name of the service</param>
        /// <remarks>
        /// Sample request:
        ///     
        ///     DELETE api/v1/discount/50/servicea
        /// </remarks>
        [Consumes("application/json")]
        [HttpDelete("{customerId:int}/{serviceName}")]
        public async Task<IActionResult> DeleteDiscount(int customerId, string serviceName)
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

            ServiceModel serviceModel = customer.UsedServices.First(x => x.ServiceName.ToUpperInvariant() == serviceName.ToUpperInvariant());
            DiscountModel discountModel = serviceModel.Discount;

            if (discountModel == null)
            {
                ModelState.AddModelError("serviceName", "There is no discount applied to the specified service.");
                return BadRequest(ModelState);
            }

            serviceModel.Discount = null;
            _dbContext.Discounts.Remove(discountModel);

            await _dbContext.SaveChangesAsync();
            return Ok();
        }

        /// <summary>
        /// Adds or updates free days for the specified customer. 
        /// Use POST to add free days to the customer, or PUT to update free days for the customer.
        /// </summary>
        /// <param name="model">The request body, parsed as a FreeDaysModel object</param>
        /// <remarks>
        /// Sample request:
        ///     
        ///     POST/PUT api/v1/discount/freedays/
        ///     {
        ///         "CustomerId": 50
        ///         "FreeDays": 100
        ///     }
        /// </remarks>
        [Consumes("application/json")]
        [HttpPost("freedays")]
        [HttpPut("freedays")]
        public async Task<IActionResult> SetFreeDays(FreeDaysModel model)
        {
            CustomerModel customer = _dbContext.Customers
                .Include(x => x.FreeDays)
                .FirstOrDefault(x => x.Id == model.CustomerId);

            if (customer == null)
            {
                ModelState.AddModelError("customerId", "The customer with the specified customer id does not exist.");
                return BadRequest(ModelState);
            }

            if (Request.Method == "POST")
            {
                if (customer.FreeDays != null)
                {
                    ModelState.AddModelError("CustomerId", "The specified customer already has free days. Use the PUT method to update the free days value.");
                    return BadRequest(ModelState);
                }

                customer.FreeDays = model;
            }
            else
            {
                if (customer.FreeDays == null)
                {
                    ModelState.AddModelError("CustomerId", "The specified customer does not have any free days. Use the POST method to add new free days.");
                    return BadRequest(ModelState);
                }

                customer.FreeDays.FreeDays = model.FreeDays;
            }

            await _dbContext.SaveChangesAsync();
            return Ok();
        }

        /// <summary>
        /// Retrieves the free days for the specified customer
        /// </summary>
        /// <param name="customerId">The customer id</param>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET api/v1/discount/freedays/50
        /// </remarks>
        [Consumes("application/json")]
        [HttpGet("freedays/{customerId:int}")]
        public IActionResult GetFreeDays(int customerId)
        {
            CustomerModel customer = _dbContext.Customers.Include(x => x.FreeDays).FirstOrDefault(x => x.Id == customerId);

            if (customer == null)
            {
                ModelState.AddModelError("customerId", "The customer with the specified id does note exist.");
                return BadRequest(ModelState);
            }

            FreeDaysModel freeDaysModel = customer.FreeDays;

            int freeDays = freeDaysModel != null ? freeDaysModel.FreeDays : 0;

            return new JsonResult(new {
                FreeDays = freeDays
            });
        }
    }
}
