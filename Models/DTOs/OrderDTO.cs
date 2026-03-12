namespace RepoApi.Models.DTOs
{
    public class OrderDto
    {
        public int Id { get; set; }

        public CustomerDTO Customer { get; set; }
        public DateTime OrderDate { get; set; }

        // Calculated in controller (not from DB directly)
        public decimal TotalPrice { get; set; }

        public List<OrderItemDto>? Items { get; set; }
    }

    public class OrderItemDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
    }
}