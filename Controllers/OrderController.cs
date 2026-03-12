using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RepoApi.Data;
using RepoApi.Models;
using RepoApi.Models.DTOs;

namespace RepoApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly RepoContext _context;

        public OrderController(RepoContext context)
        {
            _context = context;
        }

        // GET: api/order
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrders()
        {
            var orders = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Product).ThenInclude(p => p.Category)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                    .ThenInclude(p => p.Brand)
                .ToListAsync();

            var result = orders.Select(o => new OrderDto
            {
                Id = o.Id,
                OrderDate = o.OrderDate,
                Customer = new CustomerDTO { Id = o.Customer.Id, Email = o.Customer.Email, Name = o.Customer.Name},
                TotalPrice = o.OrderItems.Sum(oi => oi.UnitPrice * oi.Quantity),
                Items = o.OrderItems.Select(oi => new OrderItemDto
                {
                    ProductId = oi.ProductId,
                    ProductName = oi.Product.Name,
                    UnitPrice = oi.UnitPrice,
                    Quantity = oi.Quantity
                }).ToList()
            });

            return Ok(result);
        }

        // GET: api/order/{id}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<OrderDto>> GetOrder(int id)
        {
            var order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o=> o.OrderItems).ThenInclude(oi => oi.Product).ThenInclude(p => p.Category)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                    .ThenInclude(p => p.Brand)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return NotFound();

            var dto = new OrderDto
            {
                Id = order.Id,
                Customer = new CustomerDTO { Id = order.Customer.Id, Email = order.Customer.Email, Name = order.Customer.Name },
                OrderDate = order.OrderDate,
                TotalPrice = order.OrderItems.Sum(oi => oi.UnitPrice * oi.Quantity),
                Items = order.OrderItems.Select(oi => new OrderItemDto
                {
                    ProductId = oi.ProductId,
                    ProductName = oi.Product.Name,
                    UnitPrice = oi.UnitPrice,
                    Quantity = oi.Quantity
                }).ToList()
            };

            return Ok(dto);
        }

        // POST: api/order
        [HttpPost]
        public async Task<ActionResult<Order>> CreateOrder([FromBody] Order order)
        {
            if (order.OrderItems == null || !order.OrderItems.Any())
                return BadRequest("Order must have at least one item.");

            foreach ( var item in order.OrderItems) {
                var product = await _context.Products.FindAsync(item.ProductId);

                if (product == null) return NotFound($"Product {item.ProductId} not found.");

                item.UnitPrice = product.Price;
                    }

            order.TotalPrice = order.OrderItems.Sum(oi => oi.UnitPrice * oi.Quantity);

            order.OrderDate = DateTime.UtcNow;
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
        }

        // PUT: api/order/{id}
        [HttpPut("{id:int}")]
        public async Task<ActionResult> UpdateOrder(int id, [FromBody] Order order)
        {
            if (id != order.Id) return BadRequest();
            // Note: UnitPrice taken from request body, not validated against DB (intentional for comparison, its fixed in CreateOrder above)
            order.TotalPrice = order.OrderItems?.Sum(oi => oi.UnitPrice * oi.Quantity) ?? 0;

            _context.Entry(order).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Orders.AnyAsync(o => o.Id == id))
                    return NotFound();
                throw;
            }

            return NoContent();
        }

        // DELETE: api/order/{id}
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null) return NotFound();

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
