using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RepoApi.Data;
using RepoApi.Models;
using RepoApi.Models.DTOs;

namespace RepoApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : Controller
    {
        private readonly RepoContext _context;

        public CustomerController(RepoContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers()
        {
            return Ok(await _context.Customers.ToListAsync());
        }
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Customer>> GetCustomer(int id)
        {
            var customer = await _context.Customers
                .Include(c => c.Orders).ThenInclude(o => o.OrderItems).ThenInclude(p=> p.Product).FirstOrDefaultAsync(c => c.Id == id);

            if (customer == null) return NotFound();
            var result = new CustomerDTO
            {
                Email = customer.Email,
                Id = customer.Id,
                Name = customer.Name,
                Orders = customer.Orders.Select( o => new OrderDto
                {
                    Id = o.Id,
                    Items =  o.OrderItems.Select( oi => new OrderItemDto 
                        {
                            Id = oi.Id,
                            ProductId = oi.ProductId,
                            ProductName = oi.Product.Name,
                            UnitPrice = oi.Product.Price,
                            Quantity = oi.Quantity       
                        }).ToList(),
                    OrderDate = o.OrderDate,
                    TotalPrice = o.OrderItems.Sum(oi => oi.UnitPrice * oi.Quantity)
                }).ToList()
            };


            return Ok(result);
        }
        [HttpPost]
        public async Task<ActionResult<Customer>> PostCustomer(Customer customer)
        {
            await _context.Customers.AddAsync(customer);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCustomer), new { id = customer.Id }, customer);
        }
        [HttpPut("{id:int}")]
        public async Task<ActionResult<Customer>> PutCustomer(int id, [FromBody] Customer customer)
        {
            if (id != customer.Id) { return BadRequest(); }

            _context.Entry(customer).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Customers.AnyAsync(p => p.Id == id))
                    return NotFound();
                throw;
            }
            return NoContent();

        }
        [HttpDelete("{id:int}")]
        public async Task<ActionResult<Customer>> DeleteCustomer(int id)
        {
            var cust = await _context.Customers.FindAsync(id);

            if (cust == null) { return NotFound(); }
            

            _context.Customers.Remove(cust);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}