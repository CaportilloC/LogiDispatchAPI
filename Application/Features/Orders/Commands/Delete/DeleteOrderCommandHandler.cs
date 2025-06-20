using Application.Contracts.Services.OrderServices;
using Application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Orders.Commands.Delete
{
    public class DeleteOrderCommandHandler : IRequestHandler<DeleteOrderCommand, WrapperResponse<bool>>
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<DeleteOrderCommandHandler> _logger;

        public DeleteOrderCommandHandler(IOrderService orderService, ILogger<DeleteOrderCommandHandler> logger)
        {
            _orderService = orderService;
            _logger = logger;
        }

        public async Task<WrapperResponse<bool>> Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _orderService.DeleteAsync(request.Id);

                if (!result)
                {
                    _logger.LogWarning("Orden con ID {OrderId} no se pudo eliminar o ya estaba eliminada.", request.Id);
                    return new WrapperResponse<bool>("No se pudo eliminar la orden.");
                }

                return new WrapperResponse<bool>(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar la orden con ID {OrderId}", request.Id);
                return new WrapperResponse<bool>($"Error al eliminar la orden: {ex.Message}");
            }
        }
    }
}
