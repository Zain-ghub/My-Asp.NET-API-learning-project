using System.Text.Json.Serialization;

namespace RepoApi.Models
{
    public class Brand
    {   
        public int Id { get; set; }
        public required string Name { get; set; }
        [JsonIgnore]
        public List<Product>? Products { get; set; }
    }
}
