using Application.Attributes.Services;
using Application.Contracts.Persistence.Common.UnitOfWork;
using Application.Contracts.Services.CustomerServices;
using Application.DTOs.Orders.Customers;
using Application.Specifications.Customers;
using AutoMapper;
using Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services.CustomerService
{
    [RegisterService(ServiceLifetime.Scoped)]
    public class CustomerService : ICustomerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<CustomerService> _logger;

        public CustomerService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<CustomerService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<List<CustomerResponse>> GetAllAsync()
        {
            var spec = new GetAllCustomersSpecification();
            var customers = await _unitOfWork.Repository<Customer>().ListAsync(spec);

            if (customers == null || !customers.Any())
            {
                _logger.LogWarning("No se encontraron clientes registrados.");
                return new List<CustomerResponse>();
            }

            var responses = _mapper.Map<List<CustomerResponse>>(customers);

            _logger.LogInformation("Se recuperaron {Count} clientes", responses.Count);
            return responses;
        }
    }

}
