using RepoApi.Models;

namespace RepoApi.Services
{
    public interface IRabbitMQPublisher
    {
        Task PublishOrderCreatedAsync(int orderId, List<OrderCreatedItem> items);
    }
}
