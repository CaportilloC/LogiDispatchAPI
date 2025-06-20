using Application.DTOs.Orders.Products;

namespace Application.Contracts.Services.ProductServices
{
    public interface IProductService
    {
        Task<List<ProductResponse>> GetAllAsync();
    }
}
