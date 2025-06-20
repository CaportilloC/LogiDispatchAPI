using Ardalis.Specification;
using Domain.Entities;

namespace Application.Specifications.Products
{
    public class GetAllProductsSpecification : Specification<Product>
    {
        public GetAllProductsSpecification()
        {
            Query.OrderBy(p => p.Name);
        }
    }
}
