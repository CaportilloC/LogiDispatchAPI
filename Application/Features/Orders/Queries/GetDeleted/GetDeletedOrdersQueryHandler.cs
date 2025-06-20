using Application.Contracts.Services.OrderServices;
using Application.DTOs.Orders;
using Application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Orders.Queries.GetDeleted
{
    public class GetDeletedOrdersQueryHandler : IRequestHandler<GetDeletedOrdersQuery, WrapperResponse<List<OrderResponse>>>
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<GetDeletedOrdersQueryHandler> _logger;

        public GetDeletedOrdersQueryHandler(IOrderService orderService, ILogger<GetDeletedOrdersQueryHandler> logger)
        {
            _orderService = orderService;
            _logger = logger;
        }

        public async Task<WrapperResponse<List<OrderResponse>>> Handle(GetDeletedOrdersQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _orderService.GetDeletedAsync();
                return new WrapperResponse<List<OrderResponse>>(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener las órdenes eliminadas.");
                return new WrapperResponse<List<OrderResponse>>($"Error al obtener eliminadas: {ex.Message}");
            }
        }
    }
}
