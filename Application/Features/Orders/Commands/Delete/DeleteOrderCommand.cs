using Application.DTOs.Orders;
using Application.Wrappers.Common;
using MediatR;

namespace Application.Features.Orders.Commands.Delete
{
    public class DeleteOrderCommand : IRequest<BaseWrapperResponse<OrderResponse>>
    {
        public Guid Id { get; set; }
    }
}
