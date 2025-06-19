using Application.DTOs.Orders;

namespace Application.Contracts.Services.OrderServices
{
    public interface IOrderService
    {
        Task<List<OrderResponse>> GetAllAsync();

        Task<List<OrderResponse>> GetDeletedAsync();

        Task<OrderResponse?> GetByIdAsync(Guid id);

        Task<OrderResponse> CreateAsync(CreateOrderRequest request);

        Task<OrderResponse> UpdateAsync(Guid id, UpdateOrderRequest request);

        Task<OrderResponse> DeleteAsync(Guid id);

        Task<OrderResponse> RestoreAsync(Guid id);

    }
}
