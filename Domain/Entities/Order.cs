using Domain.Entities.Common;
using Domain.Enums;

namespace Domain.Entities
{
    public class Order: BaseEntity
    {
        public Guid Id { get; set; }

        public Guid CustomerId { get; set; }

        public decimal OriginLatitude { get; set; }
        public decimal OriginLongitude { get; set; }

        public decimal DestinationLatitude { get; set; }
        public decimal DestinationLongitude { get; set; }

        public decimal DistanceKm { get; set; }
        public decimal ShippingCostUSD { get; set; }

        public OrderStatus Status { get; set; } = OrderStatus.Created;

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        // Navigation
        public Customer Customer { get; set; } = default!;
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
