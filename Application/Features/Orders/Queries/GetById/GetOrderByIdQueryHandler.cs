using Application.Contracts.Services.OrderServices;
using Application.DTOs.Orders;
using Application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Orders.Queries.GetById
{
    public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, WrapperResponse<OrderResponse>>
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<GetOrderByIdQueryHandler> _logger;

        public GetOrderByIdQueryHandler(IOrderService orderService, ILogger<GetOrderByIdQueryHandler> logger)
        {
            _orderService = orderService;
            _logger = logger;
        }

        public async Task<WrapperResponse<OrderResponse>> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var order = await _orderService.GetByIdAsync(request.Id);

                if (order == null)
                {
                    _logger.LogWarning("Orden con ID {OrderId} no encontrada.", request.Id);
                    return new WrapperResponse<OrderResponse>("Orden no encontrada.");
                }

                return new WrapperResponse<OrderResponse>(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la orden con ID {OrderId}", request.Id);
                return new WrapperResponse<OrderResponse>($"Error al obtener la orden: {ex.Message}");
            }
        }
    }
}
