using Application.Contracts.Services.OrderServices;
using Application.DTOs.Orders;
using Application.Wrappers;
using Application.Wrappers.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Orders.Commands.Delete
{
    public class DeleteOrderCommandHandler : IRequestHandler<DeleteOrderCommand, BaseWrapperResponse<OrderResponse>>
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<DeleteOrderCommandHandler> _logger;

        public DeleteOrderCommandHandler(IOrderService orderService, ILogger<DeleteOrderCommandHandler> logger)
        {
            _orderService = orderService;
            _logger = logger;
        }

        public async Task<BaseWrapperResponse<OrderResponse>> Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _orderService.DeleteAsync(request.Id);
                return new WrapperResponse<OrderResponse>(result, "Orden eliminada correctamente.");
            }
            catch (Exception ex)
            {
                var message = ex.InnerException?.Message ?? ex.Message;
                _logger.LogError(ex, "Error al eliminar la orden {OrderId}", request.Id);
                return new WrapperResponse<OrderResponse>($"Error: {message}");
            }
        }
    }
}
