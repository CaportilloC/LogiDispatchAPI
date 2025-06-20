using Application.DTOs.Orders;
using Application.Wrappers;
using MediatR;

namespace Application.Features.Orders.Queries.GetDeleted
{
    public class GetDeletedOrdersQuery : IRequest<WrapperResponse<List<OrderResponse>>>
    {
    }
}
