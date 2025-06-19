namespace Application.DTOs.Orders
{
    public class OrderResponse
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;

        public decimal OriginLatitude { get; set; }
        public decimal OriginLongitude { get; set; }
        public decimal DestinationLatitude { get; set; }
        public decimal DestinationLongitude { get; set; }

        public decimal DistanceKm { get; set; }
        public decimal ShippingCost { get; set; }
        public string Status { get; set; } = string.Empty;

        public List<OrderItemDto> Items { get; set; } = new();
        public DateTime CreatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
