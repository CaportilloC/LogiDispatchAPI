using Application.Contracts.Services.OrderServices;
using Application.DTOs.Orders;
using Application.Wrappers;
using Application.Wrappers.Common;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Orders.Commands.Update
{
    public class UpdateOrderCommandHandler : IRequestHandler<UpdateOrderCommand, BaseWrapperResponse<OrderResponse>>
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<UpdateOrderCommandHandler> _logger;

        public UpdateOrderCommandHandler(IOrderService orderService, ILogger<UpdateOrderCommandHandler> logger)
        {
            _orderService = orderService;
            _logger = logger;
        }

        public async Task<BaseWrapperResponse<OrderResponse>> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var updateRequest = new UpdateOrderRequest
                {
                    Id = request.Id,
                    CustomerId = request.CustomerId,
                    OriginLatitude = (decimal)request.OriginLatitude,
                    OriginLongitude = (decimal)request.OriginLongitude,
                    DestinationLatitude = (decimal)request.DestinationLatitude,
                    DestinationLongitude = (decimal)request.DestinationLongitude,
                    Items = request.Items
                };

                var result = await _orderService.UpdateAsync(updateRequest);
                return new WrapperResponse<OrderResponse>(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar la orden con ID {OrderId}", request.Id);
                return new WrapperResponse<OrderResponse>($"Error al actualizar: {ex.Message}");
            }
        }
    }
}
