using Domain.Entities.Common;

namespace Domain.Entities
{
    public class ShippingCostRange : BaseEntity
    {
        public int Id { get; set; }

        public decimal MinKm { get; set; }
        public decimal MaxKm { get; set; }

        public decimal CostUSD { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
