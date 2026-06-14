using Microsoft.AspNetCore.Mvc;
using RepoApi.Models;
using RepoApi.Models.DTOs;
using RepoApi.Services;

namespace RepoApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }
        // GET: api/order
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrders()
        {
            var result = await _orderService.GetAllOrdersAsync();

            return Ok(result);
        }

        // GET: api/order/{id}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<OrderDto>> GetOrder(int id)
        {
            

            var order = await _orderService.GetOrderByIdAsync(id);

            if (order == null) return NotFound();

            return Ok(order);
        }

        // POST: api/order
        [HttpPost]
        public async Task<ActionResult<Order>> CreateOrder([FromBody] Order order)
        {
            if (order.OrderItems == null || !order.OrderItems.Any())
                return BadRequest("Order must have at least one item.");

            var result = await _orderService.CreateOrderAsync(order);
            
            if (result == null) return BadRequest("Failed to create order.");

            return CreatedAtAction(nameof(GetOrder), new { id = result.Id }, result);
        }

        // PUT: api/order/{id}
        [HttpPut("{id:int}")]
        public async Task<ActionResult> UpdateOrder(int id, [FromBody] Order order)
        {
            if (id != order.Id) return BadRequest();
            // Note: UnitPrice taken from request body, not validated against DB (intentional for comparison, its fixed in CreateOrder above)
            var result = await _orderService.UpdateOrderAsync(order);

            if (result == false) return NotFound();

            return NoContent();
        }

        // DELETE: api/order/{id}
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteOrder(int id)
        {
            if (id <= 0) return BadRequest("Invalid order ID.");
            var result = await _orderService.DeleteOrderAsync(id);
            if (result == false) return NotFound();
            return NoContent();
        }
    }
}
