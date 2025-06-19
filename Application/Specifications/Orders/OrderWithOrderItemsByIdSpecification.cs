using Ardalis.Specification;
using Domain.Entities;

namespace Application.Specifications.Orders
{
    public class OrderWithOrderItemsByIdSpecification: Specification<Order>
    {
        public OrderWithOrderItemsByIdSpecification(Guid id)
        {
            Query
                .Where(o => o.Id == id && o.DeletedAt == null)
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product);
        }
    }
}
