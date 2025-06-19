using Application.DTOs.Orders;
using Application.Wrappers.Common;
using MediatR;

namespace Application.Features.Orders.Commands.Update
{
    public class UpdateOrderCommand : IRequest<BaseWrapperResponse<OrderResponse>>
    {
        public Guid Id { get; set; }

        public Guid CustomerId { get; set; }
        public List<OrderItemDto> Items { get; set; } = new();
        public double OriginLatitude { get; set; }
        public double OriginLongitude { get; set; }
        public double DestinationLatitude { get; set; }
        public double DestinationLongitude { get; set; }
    }
}
