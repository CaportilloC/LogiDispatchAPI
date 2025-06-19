using Application.DTOs.Orders;
using Application.Wrappers.Common;
using MediatR;

namespace Application.Features.Orders.Queries.GetDeleted
{
    public class GetDeletedOrdersQuery : IRequest<BaseWrapperResponse<List<OrderResponse>>>
    {
    }
}
