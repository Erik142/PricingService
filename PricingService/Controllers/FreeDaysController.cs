using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PricingService.Database;
using PricingService.Model;

namespace PricingService.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class FreeDaysController : ControllerBase
    {
        private PricingDbContext _dbContext;

        public FreeDaysController(PricingDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Adds or updates free days for the specified customer. 
        /// Use POST to add free days to the customer, or PUT to update free days for the customer.
        /// </summary>
        /// <param name="model">The request body, parsed as a FreeDaysModel object</param>
        /// <remarks>
        /// Sample request:
        ///     
        ///     POST/PUT api/v1/freedays/
        ///     {
        ///         "CustomerId": 50
        ///         "FreeDays": 100
        ///     }
        /// </remarks>
        [Consumes("application/json")]
        [HttpPost]
        [HttpPut]
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
        ///     GET api/v1/freedays/50
        /// </remarks>
        [Consumes("application/json")]
        [HttpGet("{customerId:int}")]
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

            return new JsonResult(new
            {
                FreeDays = freeDays
            });
        }
    }
}
