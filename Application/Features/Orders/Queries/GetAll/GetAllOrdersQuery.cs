using Application.DTOs.Orders;
using Application.Wrappers.Common;
using MediatR;

namespace Application.Features.Orders.Queries.GetAll
{
    public class GetAllOrdersQuery : IRequest<BaseWrapperResponse<List<OrderResponse>>>
    {
    }
}
