using Application.Attributes.Services;
using Application.Contracts.Persistence.Common.UnitOfWork;
using Application.Contracts.Services.ProductServices;
using Application.DTOs.Orders.Products;
using Application.Specifications.Products;
using AutoMapper;
using Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services.ProductService
{
    [RegisterService(ServiceLifetime.Scoped)]
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<ProductService> _logger;

        public ProductService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<ProductService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<List<ProductResponse>> GetAllAsync()
        {
            var spec = new GetAllProductsSpecification();
            var products = await _unitOfWork.Repository<Product>().ListAsync(spec);

            if (products == null || !products.Any())
            {
                _logger.LogWarning("No se encontraron productos registrados.");
                return new List<ProductResponse>();
            }

            var responses = _mapper.Map<List<ProductResponse>>(products);

            _logger.LogInformation("Se recuperaron {Count} productos", responses.Count);
            return responses;
        }
    }
}
