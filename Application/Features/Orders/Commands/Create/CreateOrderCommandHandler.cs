using Application.Contracts.Services.OrderServices;
using Application.DTOs.Orders;
using Application.Wrappers;
using Application.Wrappers.Common;
using AutoMapper;
using MediatR;

namespace Application.Features.Orders.Commands.Create
{
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, BaseWrapperResponse<OrderResponse>>
    {
        private readonly IOrderService _orderService;
        private readonly IMapper _mapper;

        public CreateOrderCommandHandler(IOrderService orderService, IMapper mapper)
        {
            _orderService = orderService;
            _mapper = mapper;
        }

        public async Task<BaseWrapperResponse<OrderResponse>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Mapear el command al DTO de entrada
                var dto = _mapper.Map<CreateOrderRequest>(request);

                var result = await _orderService.CreateAsync(dto);

                return new WrapperResponse<OrderResponse>(result, "Orden creada correctamente.");
            }
            catch (Exception ex)
            {
                var message = ex.InnerException?.Message ?? ex.Message;
                return new WrapperResponse<OrderResponse>($"Error al crear la orden: {message}");
            }
        }
    }
}
