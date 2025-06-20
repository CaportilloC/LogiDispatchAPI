using Ardalis.Specification;
using Domain.Entities;

namespace Application.Specifications.Customers
{
    public class GetAllCustomersSpecification : Specification<Customer>
    {
        public GetAllCustomersSpecification()
        {
            Query.OrderBy(c => c.Name);
        }
    }
}
