using Application.DTOs.Orders.Products;
using Application.Wrappers;
using MediatR;

namespace Application.Features.Orders.Queries.GetAll
{
    public class GetAllProductsQuery : IRequest<WrapperResponse<List<ProductResponse>>>
    {
    }
}
