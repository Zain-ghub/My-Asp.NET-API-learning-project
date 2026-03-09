using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RepoApi.Data;
using RepoApi.Models;
using RepoApi.Models.DTOs;

namespace RepoApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderItemController : ControllerBase
    {
        private readonly RepoContext _context;

        public OrderItemController(RepoContext context)
        {
            _context = context;
        }

  
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderItemDto>>> GetOrderItems()
        {
            var orderItems = await _context.OrderItems
                .Include(o => o.Order)
                .Include(o => o.Product)
                .ToListAsync();

            var result = orderItems.Select(oi => new OrderItemDto
            {
                Id = oi.Id,
                ProductId = oi.ProductId,
                    ProductName = oi.Product.Name,
                    UnitPrice = oi.UnitPrice,
                    Quantity = oi.Quantity           
            });

            return Ok(result);
        }


        [HttpGet("{id:int}")]
        public async Task<ActionResult<OrderItemDto>> GetOrderItem(int id)
        {
            var orderItem = await _context.OrderItems
                .Include(oi => oi.Order)
                .Include(oi => oi.Product)
                .FirstOrDefaultAsync(oi => oi.Id == id);

            if (orderItem == null) return NotFound();

            var dto = new OrderItemDto
            {
                    Id = orderItem.Id,
                    ProductId = orderItem.ProductId,
                    ProductName = orderItem.Product.Name,
                    UnitPrice = orderItem.UnitPrice,
                    Quantity = orderItem.Quantity

            };

            return Ok(dto);
        }


        [HttpPost]
        public async Task<ActionResult<OrderItemDto>> CreateOrderItem([FromBody] OrderItem orderItem)
        {
            _context.OrderItems.Add(orderItem);

            await _context.SaveChangesAsync();

            var dto = new OrderItemDto
            {
                Id = orderItem.Id,
                ProductId = orderItem.ProductId,
                ProductName = (await _context.Products.FindAsync(orderItem.ProductId))?.Name ?? string.Empty,
                UnitPrice = orderItem.UnitPrice,
                Quantity = orderItem.Quantity
            };

            return CreatedAtAction(nameof(GetOrderItem), new { id = orderItem.Id }, dto);
        }


        [HttpPut("{id:int}")]
        public async Task<ActionResult> UpdateOrderItem(int id, [FromBody] OrderItem orderItem)
        {
            if (id != orderItem.Id) return BadRequest();

           
            _context.Entry(orderItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.OrderItems.AnyAsync(o => o.Id == id))
                    return NotFound();
                throw;
            }

            return NoContent();
        }

    
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteOrderItem(int id)
        {
            var orderItem = await _context.OrderItems.FindAsync(id);
            if (orderItem == null) return NotFound();

            _context.OrderItems.Remove(orderItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
