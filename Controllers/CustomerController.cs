using Microsoft.AspNetCore.Mvc;

using RepoApi.Services;
using RepoApi.Models;


namespace RepoApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : Controller
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService CustomerService)
        {
            _customerService = CustomerService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers()
        {
            return Ok(await _customerService.GetAllCustomersAsync());
        }
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Customer>> GetCustomer(int id)
        {
            var result = await _customerService.GetCustomerByIdAsync(id);

            if (result == null) { return NotFound(); }

            return Ok(result);
        }
        [HttpPost]
        public async Task<ActionResult<Customer>> PostCustomer(Customer customer)
        {
            var result =  await _customerService.CreateCustomerAsync(customer);

            if (result == null) { return BadRequest(); }

            return CreatedAtAction(nameof(GetCustomer), new { id = result.Id }, result);
        }
        [HttpPut("{id:int}")]
        public async Task<ActionResult<Customer>> PutCustomer(int id, [FromBody] Customer customer)
        {
           var result  = await _customerService.UpdateCustomerAsync(customer, id);

            if (result == null) { return NotFound(); }

            return NoContent();

        }
        [HttpDelete("{id:int}")]
        public async Task<ActionResult<Customer>> DeleteCustomer(int id)
        {
            var res = await _customerService.DeleteCustomerAsync(id);
            
                if (res == false) { return NotFound(); }

            return NoContent();
        }
    }
}