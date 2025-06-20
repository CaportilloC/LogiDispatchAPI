using Application.Contracts.Services.CustomerServices;
using Application.DTOs.Orders.Customers;
using Application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Orders.Queries.GetAll
{
    public class GetAllCustomersQueryHandler : IRequestHandler<GetAllCustomersQuery, WrapperResponse<List<CustomerResponse>>>
    {
        private readonly ICustomerService _customerService;
        private readonly ILogger<GetAllCustomersQueryHandler> _logger;

        public GetAllCustomersQueryHandler(ICustomerService customerService, ILogger<GetAllCustomersQueryHandler> logger)
        {
            _customerService = customerService;
            _logger = logger;
        }

        public async Task<WrapperResponse<List<CustomerResponse>>> Handle(GetAllCustomersQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _customerService.GetAllAsync();
                return new WrapperResponse<List<CustomerResponse>>(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el listado de clientes.");
                return new WrapperResponse<List<CustomerResponse>>($"Error al obtener el listado: {ex.Message}");
            }
        }
    }
}
