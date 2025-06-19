using Application.Contracts.Services.OrderServices;
using Application.DTOs.Orders;
using Application.Wrappers;
using Application.Wrappers.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Orders.Queries.GetById
{
    public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, BaseWrapperResponse<OrderResponse>>
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<GetOrderByIdQueryHandler> _logger;

        public GetOrderByIdQueryHandler(IOrderService orderService, ILogger<GetOrderByIdQueryHandler> logger)
        {
            _orderService = orderService;
            _logger = logger;
        }

        public async Task<BaseWrapperResponse<OrderResponse>> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _orderService.GetByIdAsync(request.Id);

                if (result == null)
                {
                    _logger.LogWarning("Orden con ID {OrderId} no encontrada.", request.Id);
                    return new WrapperResponse<OrderResponse>("La orden no existe.");
                }

                return new WrapperResponse<OrderResponse>(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la orden con ID {OrderId}", request.Id);
                return new WrapperResponse<OrderResponse>($"Error al consultar la orden: {ex.Message}");
            }
        }
    }
}
