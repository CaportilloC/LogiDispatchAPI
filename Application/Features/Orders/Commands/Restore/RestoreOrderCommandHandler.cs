using Application.Contracts.Services.OrderServices;
using Application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Orders.Commands.Restore
{
    public class RestoreOrderCommandHandler : IRequestHandler<RestoreOrderCommand, WrapperResponse<bool>>
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<RestoreOrderCommandHandler> _logger;

        public RestoreOrderCommandHandler(IOrderService orderService, ILogger<RestoreOrderCommandHandler> logger)
        {
            _orderService = orderService;
            _logger = logger;
        }

        public async Task<WrapperResponse<bool>> Handle(RestoreOrderCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _orderService.RestoreAsync(request.Id);

                if (!result)
                {
                    _logger.LogWarning("Orden con ID {OrderId} no se pudo restaurar o no estaba eliminada.", request.Id);
                    return new WrapperResponse<bool>("No se pudo restaurar la orden.");
                }

                return new WrapperResponse<bool>(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al restaurar la orden con ID {OrderId}", request.Id);
                return new WrapperResponse<bool>($"Error al restaurar la orden: {ex.Message}");
            }
        }
    }
}
