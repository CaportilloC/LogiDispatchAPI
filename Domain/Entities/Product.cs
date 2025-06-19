namespace Domain.Entities
{
    public class Product
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = default!;
        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; }

        // Navigation
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
