using RepoApi.Models.DTOs;
using RepoApi.Models;

namespace RepoApi.Services
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderDto>> GetAllOrdersAsync();
        Task<OrderDto?> GetOrderByIdAsync(int id);
        Task<OrderDto?> CreateOrderAsync(Order order);
        Task<bool> UpdateOrderAsync(Order order, int id);
        Task<bool> DeleteOrderAsync(int id);
    }
}
