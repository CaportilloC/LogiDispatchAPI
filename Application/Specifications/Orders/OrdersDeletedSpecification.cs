using Ardalis.Specification;
using Domain.Entities;

namespace Application.Specifications.Orders
{
    public class OrdersDeletedSpecification : Specification<Order>
    {
        public OrdersDeletedSpecification()
        {
            Query
                .Where(o => o.DeletedAt != null)
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .OrderByDescending(o => o.DeletedAt);
        }
    }
}
