namespace Application.DTOs.Orders
{
    public class CreateOrderRequest
    {
        public Guid CustomerId { get; set; }
        public List<OrderItemDto> Items { get; set; } = new();
        public decimal OriginLatitude { get; set; }
        public decimal OriginLongitude { get; set; }
        public decimal DestinationLatitude { get; set; }
        public decimal DestinationLongitude { get; set; }
    }
}
