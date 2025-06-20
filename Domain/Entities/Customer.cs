using Domain.Entities.Common;

namespace Domain.Entities
{
    public class Customer : BaseEntity
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = default!;
        public string? Email { get; set; }
        public string? Phone { get; set; }

        public DateTime CreatedAt { get; set; }

        // Navigation
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
