using RepoApi.Models.DTOs;
using RepoApi.Data;
using RepoApi.Models;
using Microsoft.EntityFrameworkCore;

namespace RepoApi.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly RepoContext _context;
        public CustomerService(RepoContext context)
        {
            _context = context;
        }
        public async Task<List<CustomerDTO>> GetAllCustomersAsync()
        {
            var customers = await _context.Customers.ToListAsync();
            return customers.Select(c => new CustomerDTO
            {
                Id = c.Id,
                Name = c.Name,
                Email = c.Email
            }).ToList();
        }
        public async Task<CustomerDTO?> GetCustomerByIdAsync(int id)
        {
            if (id <= 0) return null;

            var customer = await _context.Customers
               .Include(c => c.Orders).ThenInclude(o => o.OrderItems).ThenInclude(p => p.Product).FirstOrDefaultAsync(c => c.Id == id);

            if (customer == null) return null;

            var result = new CustomerDTO
            {
                Email = customer.Email,
                Id = customer.Id,
                Name = customer.Name,
                Orders = customer.Orders.Select(o => new OrderDto
                {
                    Id = o.Id,
                    Items = o.OrderItems.Select(oi => new OrderItemDto
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
            return result;
        }
        public async Task<Customer?> CreateCustomerAsync(Customer newCustomer)
        {
            var customer = new Customer
            {
                Name = newCustomer.Name,
                Email = newCustomer.Email,

            };

            await _context.Customers.AddAsync(customer);
            await _context.SaveChangesAsync();

            return customer;
        }
        public async Task<Customer?> UpdateCustomerAsync(Customer customer, int id)
        {
            if (id != customer.Id) { return null; }

            _context.Entry(customer).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Customers.AnyAsync(p => p.Id == id))
                    return null;
                throw;
            }

            return customer;
        }
        public async Task<bool> DeleteCustomerAsync(int id)
        {
            var customer = await _context.Customers.FindAsync(id);

            if (customer == null) { return false; }

            _context.Customers.Remove(customer);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}
