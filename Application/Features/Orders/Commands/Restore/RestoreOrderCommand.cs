using Application.DTOs.Orders;
using Application.Wrappers.Common;
using MediatR;

namespace Application.Features.Orders.Commands.Restore
{
    public class RestoreOrderCommand : IRequest<BaseWrapperResponse<OrderResponse>>
    {
        public Guid Id { get; set; }
    }
}
