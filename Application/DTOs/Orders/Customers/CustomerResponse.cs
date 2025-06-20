namespace Application.DTOs.Orders.Customers
{
    public class CustomerResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
    }
}
