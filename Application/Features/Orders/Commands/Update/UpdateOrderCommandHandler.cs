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
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateOrderCommandHandler> _logger;

        public UpdateOrderCommandHandler(
            IOrderService orderService,
            IMapper mapper,
            ILogger<UpdateOrderCommandHandler> logger)
        {
            _orderService = orderService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<BaseWrapperResponse<OrderResponse>> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var dto = _mapper.Map<UpdateOrderRequest>(request);
                var result = await _orderService.UpdateAsync(request.Id, dto);

                return new WrapperResponse<OrderResponse>(result, "Orden actualizada correctamente.");
            }
            catch (Exception ex)
            {
                var message = ex.InnerException?.Message ?? ex.Message;
                _logger.LogError(ex, "Error al actualizar la orden {OrderId}", request.Id);
                return new WrapperResponse<OrderResponse>($"Error: {message}");
            }
        }
    }
}
