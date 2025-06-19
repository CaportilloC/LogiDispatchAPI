using Application.Contracts.Services.OrderServices;
using Application.DTOs.Orders;
using Application.Wrappers.Common;
using Application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Orders.Commands.Restore
{
    public class RestoreOrderCommandHandler : IRequestHandler<RestoreOrderCommand, BaseWrapperResponse<OrderResponse>>
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<RestoreOrderCommandHandler> _logger;

        public RestoreOrderCommandHandler(IOrderService orderService, ILogger<RestoreOrderCommandHandler> logger)
        {
            _orderService = orderService;
            _logger = logger;
        }

        public async Task<BaseWrapperResponse<OrderResponse>> Handle(RestoreOrderCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _orderService.RestoreAsync(request.Id);
                return new WrapperResponse<OrderResponse>(result, "Orden restaurada correctamente.");
            }
            catch (Exception ex)
            {
                var message = ex.InnerException?.Message ?? ex.Message;
                _logger.LogError(ex, "Error al restaurar la orden {OrderId}", request.Id);
                return new WrapperResponse<OrderResponse>($"Error: {message}");
            }
        }
    }
}
