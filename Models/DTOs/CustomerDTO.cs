namespace RepoApi.Models.DTOs
{
    public class CustomerDTO
    {
        public int Id { get; set; } 
        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public List<OrderDto>? Orders { get; set; }

    }
}
