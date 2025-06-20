using Application.DTOs.Orders;
using Application.Wrappers;
using Application.Wrappers.Common;
using MediatR;

namespace Application.Features.Orders.Queries.GetAll
{
    public class GetAllOrdersQuery : IRequest<WrapperResponse<List<OrderResponse>>>
    {
    }
}
