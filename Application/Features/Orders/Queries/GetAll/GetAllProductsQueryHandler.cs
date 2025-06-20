using Application.Contracts.Services.ProductServices;
using Application.DTOs.Orders.Products;
using Application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Orders.Queries.GetAll
{
    public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, WrapperResponse<List<ProductResponse>>>
    {
        private readonly IProductService _productService;
        private readonly ILogger<GetAllProductsQueryHandler> _logger;

        public GetAllProductsQueryHandler(IProductService productService, ILogger<GetAllProductsQueryHandler> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        public async Task<WrapperResponse<List<ProductResponse>>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _productService.GetAllAsync();
                return new WrapperResponse<List<ProductResponse>>(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el listado de productos.");
                return new WrapperResponse<List<ProductResponse>>($"Error al obtener el listado: {ex.Message}");
            }
        }
    }
}
