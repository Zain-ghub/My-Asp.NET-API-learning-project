using System.ComponentModel.DataAnnotations.Schema;

namespace RepoApi.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public Customer ?Customer { get; set; }
        public DateTime OrderDate { get; set; }

        public decimal TotalPrice { get; set; }
        public List<OrderItem> ?OrderItems { get; set; }
    }
}
