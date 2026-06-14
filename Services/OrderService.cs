using RepoApi.Models.DTOs;
using RepoApi.Data;
using RepoApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

namespace RepoApi.Services
{
    public class OrderService : IOrderService
    {
        private readonly RepoContext _context;
        
        public OrderService(RepoContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<OrderDto>> GetAllOrdersAsync()
        {
            var orders = await _context.Orders
              .Include(o => o.Customer)
              .Include(o => o.OrderItems).ThenInclude(oi => oi.Product).ThenInclude(p => p.Category)
                .Include(o => o.OrderItems)
                  .ThenInclude(oi => oi.Product)
                 .ThenInclude(p => p.Brand)
               .ToListAsync();

            return orders.Select(o => new OrderDto
            {
                Id = o.Id,
                OrderDate = o.OrderDate,
                Customer = new CustomerDTO { Id = o.Customer.Id, Email = o.Customer.Email, Name = o.Customer.Name },
                TotalPrice = o.OrderItems.Sum(oi => oi.UnitPrice * oi.Quantity),
                Items = o.OrderItems.Select(oi => new OrderItemDto
                {
                    ProductId = oi.ProductId,
                    ProductName = oi.Product.Name,
                    UnitPrice = oi.UnitPrice,
                    Quantity = oi.Quantity
                }).ToList()
            });

        }
        public async Task<OrderDto?> GetOrderByIdAsync(int id)
        {
            var order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Product).ThenInclude(p => p.Category)
                    .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                     .ThenInclude(p => p.Brand)
                 .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return null;

            var dto = new OrderDto
            {
                Id = order.Id,
                OrderDate = order.OrderDate,
                Customer = new CustomerDTO { Id = order.Customer.Id, Email = order.Customer.Email, Name = order.Customer.Name },
                TotalPrice = order.OrderItems.Sum(oi => oi.UnitPrice * oi.Quantity),
                Items = order.OrderItems.Select(oi => new OrderItemDto
                {
                    ProductId = oi.ProductId,
                    ProductName = oi.Product.Name,
                    UnitPrice = oi.UnitPrice,
                    Quantity = oi.Quantity
                }).ToList()
                };

            return dto;
        }

        public async Task<Order?> CreateOrderAsync(Order order)
        {
            foreach (var item in order.OrderItems)
            {
                var product = await _context.Products.FindAsync(item.ProductId);

                if (product == null) return null;

                item.UnitPrice = product.Price;
            }

            order.TotalPrice = order.OrderItems.Sum(oi => oi.UnitPrice * oi.Quantity);

            order.OrderDate = DateTime.UtcNow;
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();

            return order;
        }

        public async Task<bool> UpdateOrderAsync(Order order, int id) 
        {
            if (id != order.Id) return false;

            order.TotalPrice = order.OrderItems?.Sum(oi => oi.UnitPrice * oi.Quantity) ?? 0;

            _context.Entry(order).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Orders.AnyAsync(o => o.Id == order.Id))
                    return false;
                throw;
            }
            return true;

        }

        public async Task<bool> DeleteOrderAsync(int id)
        {

            var order = await _context.Orders.FindAsync(id);
            if (order == null) return false;

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return true;
        }

    }
}
