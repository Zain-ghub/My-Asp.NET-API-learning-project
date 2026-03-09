using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace RepoApi.Models
{
    public class Product
    {
        public int Id { get; set; }

        public required string Name { get; set; }

        public decimal Price { get; set; }

        public decimal Weight { get; set; }

        public int CategoryId { get; set; }
        
        public Category ?Category { get; set; }
        public int BrandId { get; set; }
        
        public Brand ?Brand { get; set; }
        [JsonIgnore]
        public List<OrderItem> ?OrderItems { get; set; }
    }
}
