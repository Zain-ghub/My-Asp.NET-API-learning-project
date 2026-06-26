using Microsoft.Identity.Client;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using RepoApi.Models;

namespace RepoApi.Services
{
    public class RabbitMQPublisher
    {
        private readonly string _hostName;

        public RabbitMQPublisher(string hostName)
        {
            _hostName = hostName;
        }

        public async Task PublishOrderCreatedAsync(int orderId,List<OrderCreatedItem> items)
        {
            var factory = new ConnectionFactory() { HostName = _hostName };
            using var connection = await factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(queue: "order_created", durable: true, exclusive: false, autoDelete: false);

            var message = JsonSerializer.Serialize(new { OrderId = orderId, Items = items });
            var body = Encoding.UTF8.GetBytes(message);

            await channel.BasicPublishAsync(exchange: "", routingKey: "order_created", body: body);
        }
    }
}
