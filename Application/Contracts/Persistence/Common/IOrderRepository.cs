using Domain.Entities;

namespace Application.Contracts.Persistence.Common
{
    public interface IOrderRepository
    {
        Task<List<Order>> GetAllAsync();
        Task<Order?> GetByIdAsync(Guid id);
    }
}
