// using EFExample.API.Dtos.Customer;
// using EFExample.Common.Data.DBFirst;
// using EFExample.Common.Data.DBFirst.Entities;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.EntityFrameworkCore;
//
// namespace EFExample.API.Controllers;
//
// [ApiController]
// [Produces("application/json")]
// [Consumes("application/json")]
// [Route("api/[controller]")]
// public class DbFirstCustomersController : ControllerBase
// {
//     private readonly ILogger<DbFirstCustomersController> _logger;
//     private readonly DbFirstDbContext _dbContext;
//     
//     public DbFirstCustomersController(ILogger<DbFirstCustomersController> logger, DbFirstDbContext dbContext)
//     {
//         _logger = logger;
//         _dbContext = dbContext;
//     }
//     
//     [HttpGet]
//     [ProducesResponseType(StatusCodes.Status200OK)]
//     [ProducesResponseType(StatusCodes.Status204NoContent)]
//     [ProducesResponseType(StatusCodes.Status500InternalServerError)]
//     public async Task<ActionResult<List<Customer>>> GetCustomers()
//     {
//         try
//         {
//             if (_logger.IsEnabled(LogLevel.Debug)) _logger.LogDebug("GetCustomers called");
//             
//             List<Customer?> customers = (await _dbContext.Customers.AsNoTracking().ToListAsync())!;
//             
//             if (customers.Count > 0) return Ok(customers);
//     
//             return NoContent();
//         }
//         catch (Exception ex)
//         {
//             if (_logger.IsEnabled(LogLevel.Error))
//             {
//                 _logger.LogError("Error fetching customers {exceptionMessage}", ex.Message);
//             }
//             
//             return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while fetching customers.");
//         }
//     }
//     
//     [HttpGet("{id}")]
//     [ProducesResponseType(StatusCodes.Status200OK)]
//     [ProducesResponseType(StatusCodes.Status404NotFound)]
//     [ProducesResponseType(StatusCodes.Status500InternalServerError)]
//     public async Task<ActionResult<Customer>> GetCustomerById(int id)
//     {
//         try
//         {
//             if (_logger.IsEnabled(LogLevel.Debug)) _logger.LogDebug("GetCustomerById called with {id}", id);
//             
//             Customer? customer = await _dbContext.Customers.AsNoTracking().SingleOrDefaultAsync(c => c.Id == id);
//     
//             if (customer is not null) return Ok(customer);
//     
//             return NotFound();
//         }
//         catch (Exception ex)
//         {
//             if (_logger.IsEnabled(LogLevel.Error))
//             {
//                 _logger.LogError("Error fetching customer {id}. {exceptionMessage}", id, ex.Message);
//             }
//             
//             return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while fetching the customer '{id}'.");
//         }
//     }
//
//     [HttpPost]
//     [ProducesResponseType(StatusCodes.Status201Created)]
//     [ProducesResponseType(StatusCodes.Status400BadRequest)]
//     [ProducesResponseType(StatusCodes.Status500InternalServerError)]
//     public async Task<ActionResult<Customer>> CreateCustomer([FromBody] CreateCustomerRequest createCustomerRequest)
//     {
//         try
//         {
//             if (_logger.IsEnabled(LogLevel.Debug)) _logger.LogDebug("CreateCustomer called");
//
//             Customer? customer = new Customer()
//             {
//                 FirstName = createCustomerRequest.FirstName,
//                 LastName = createCustomerRequest.LastName,
//                 Email = createCustomerRequest.Email,
//                 Phone = createCustomerRequest.Phone
//             };
//             
//             _dbContext.Customers.Add(customer);
//             await _dbContext.SaveChangesAsync();
//
//             return CreatedAtAction(nameof(GetCustomerById), new { id = customer.Id }, customer);
//         }
//         catch (Exception ex)
//         {
//             if (_logger.IsEnabled(LogLevel.Error))
//             {
//                 _logger.LogError("Error creating customer {exceptionMessage}", ex.Message);
//             }
//             
//             return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while creating the customer");
//         }
//     }
//     
//     [HttpDelete("{id}")]
//     [ProducesResponseType(StatusCodes.Status200OK)]
//     [ProducesResponseType(StatusCodes.Status404NotFound)]
//     [ProducesResponseType(StatusCodes.Status500InternalServerError)]
//     public async Task<ActionResult> DeleteCustomer([FromRoute] int id)
//     {
//         try
//         {
//             if (_logger.IsEnabled(LogLevel.Debug)) _logger.LogDebug("Deleting Customer {id}", id);
//
//             Customer? customer = await _dbContext.Customers.SingleOrDefaultAsync(c => c.Id == id);
//
//             if (customer is not null)
//             {
//                 _dbContext.Customers.Remove(customer);
//                 await _dbContext.SaveChangesAsync();
//
//                 return Ok();
//             }
//
//             return NotFound();
//         }
//         catch (Exception ex)
//         {
//             if (_logger.IsEnabled(LogLevel.Error))
//             {
//                 _logger.LogError("Error deleting customer {id} {exceptionMessage}", id, ex.Message);
//             }
//             
//             return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while deleting customer {id}");
//         }
//     }
// }