namespace Domain.Entities
{
    public class ShippingCostRange
    {
        public int Id { get; set; }

        public decimal MinKm { get; set; }
        public decimal MaxKm { get; set; }

        public decimal CostUSD { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
