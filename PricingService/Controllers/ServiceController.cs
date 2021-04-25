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
    public class ServiceController : Controller
    {
        private PricingDbContext _dbContext;
        private ILogger<ServiceController> _logger;

        public ServiceController(ILogger<ServiceController> logger, PricingDbContext dbContext)
        {
            this._dbContext = dbContext;
            this._logger = logger;
        }

        /// <summary>
        /// Add a service to the customer with the id customerId or update an existing service for the customer. Use POST to add a new service, or PUT to update an existing service.
        /// </summary>
        /// <param name="customerId">The id for the customer</param>
        /// <param name="service">The model, including at least the name of the service and the start date for the service</param>
        /// <remarks>
        /// Sample request (POST):
        /// 
        ///     POST api/v1/service/50
        ///     {
        ///         "ServiceName": "ServiceA",
        ///         "StartDate": "2015-01-01",
        ///         "EndDate": "2020-12-31"
        ///     }
        /// Sample request (PUT):
        /// 
        ///     PUT api/v1/service/50
        ///     {
        ///         "ServiceName": "ServiceA",
        ///         "StartDate": "2017-04-01",
        ///     }
        /// </remarks>
        [HttpPost("{customerId:int}")]
        [HttpPut("{customerId:int}")]
        public async Task<IActionResult> AddService(int customerId, ServiceModel service)
        {
            _logger.LogDebug(Request.Method);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            service.CustomerId = customerId;

            CustomerModel customer = _dbContext.Customers.Include(x => x.UsedServices).Include(x => x.FreeDays).FirstOrDefault(x => x.Id == customerId);
            ServiceDefinitionModel serviceDefinition = _dbContext.ServiceDefinitions.Include(x => x.BillingDays).FirstOrDefault(x => x.ServiceName == service.ServiceName);

            if (customer == null || customer.UsedServices == null || (!customer.UsedServices.Any(x => x.ServiceName == service.ServiceName && Request.Method == "POST") || Request.Method == "PUT"))
            {
                if (serviceDefinition == null)
                {
                    return new JsonResult(new
                    {
                        Error = $"The name '{service.ServiceName}' is not a valid service."
                    });
                }

                if (customer == null)
                {
                    customer = new CustomerModel()
                    {
                        Id = customerId
                    };

                    _dbContext.Customers.Add(customer);

                    _dbContext.Database.OpenConnection();
                    _dbContext.Database.ExecuteSqlRaw("SET IDENTITY_INSERT dbo.Customers ON");
                    _dbContext.SaveChanges();
                    _dbContext.Database.ExecuteSqlRaw("SET IDENTITY_INSERT dbo.Customers OFF");
                }

                if (customer.UsedServices == null)
                {
                    customer.UsedServices = new List<ServiceModel>();
                }

                if (Request.Method == "POST")
                {
                    service.CustomerId = customerId;

                    if (service.Price == 0)
                    {
                        service.Price = serviceDefinition.Price;
                    }

                    service.BillingDays = serviceDefinition.BillingDays.Select(x => x.Clone()).ToArray();

                    customer.UsedServices.Add(service);
                }
                else
                {
                    ServiceModel oldService = customer.UsedServices.Find(x => x.ServiceName == service.ServiceName);
                    oldService.Price = service.Price > 0 ? service.Price : oldService.Price;
                    oldService.StartDate = service.StartDate != null ? service.StartDate : oldService.StartDate;
                    oldService.EndDate = service.EndDate != null ? service.EndDate : oldService.EndDate;
                }
            }
            else
            {
                return new JsonResult(new
                {
                    Error = $"The customer with the id {customerId} is already using the service \"{service.ServiceName}\"."
                });
            }

            await _dbContext.SaveChangesAsync();
            return Ok();
        }
    }
}
