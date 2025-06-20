using Application.Contracts.Services.OrderServices;
using Application.DTOs.Orders;
using Application.Wrappers;
using Application.Wrappers.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Orders.Queries.GetAll
{
    public class GetAllOrdersQueryHandler : IRequestHandler<GetAllOrdersQuery, WrapperResponse<List<OrderResponse>>>
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<GetAllOrdersQueryHandler> _logger;

        public GetAllOrdersQueryHandler(IOrderService orderService, ILogger<GetAllOrdersQueryHandler> logger)
        {
            _orderService = orderService;
            _logger = logger;
        }

        public async Task<WrapperResponse<List<OrderResponse>>> Handle(GetAllOrdersQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _orderService.GetAllAsync();
                return new WrapperResponse<List<OrderResponse>>(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el listado de órdenes.");
                return new WrapperResponse<List<OrderResponse>>($"Error al obtener el listado: {ex.Message}");
            }
        }
    }
}
