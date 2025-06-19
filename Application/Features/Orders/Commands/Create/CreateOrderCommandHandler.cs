using Application.Contracts.Services.OrderServices;
using Application.DTOs.Orders;
using Application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Orders.Commands.Create
{
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, WrapperResponse<OrderResponse>>
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<CreateOrderCommandHandler> _logger;

        public CreateOrderCommandHandler(IOrderService orderService, ILogger<CreateOrderCommandHandler> logger)
        {
            _orderService = orderService;
            _logger = logger;
        }

        public async Task<WrapperResponse<OrderResponse>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var createRequest = new CreateOrderRequest
                {
                    CustomerId = request.CustomerId,
                    OriginLatitude = (decimal)request.OriginLatitude,
                    OriginLongitude = (decimal)request.OriginLongitude,
                    DestinationLatitude = (decimal)request.DestinationLatitude,
                    DestinationLongitude = (decimal)request.DestinationLongitude,
                    Items = request.Items
                };

                var result = await _orderService.CreateAsync(createRequest);
                return new WrapperResponse<OrderResponse>(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear una nueva orden.");
                return new WrapperResponse<OrderResponse>($"Error al crear la orden: {ex.Message}");
            }
        }
    }
}
