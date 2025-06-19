using Ardalis.Specification;
using Domain.Entities;

namespace Application.Specifications.Orders
{
    public class OrderWithOrderItemsSpecification : Specification<Order>
    {
        public OrderWithOrderItemsSpecification()
        {
            Query
                .Where(o => o.DeletedAt == null)
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .OrderByDescending(o => o.CreatedAt);
        }
    }
}
