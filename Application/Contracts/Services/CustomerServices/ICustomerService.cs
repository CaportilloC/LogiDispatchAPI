using Application.DTOs.Orders.Customers;

namespace Application.Contracts.Services.CustomerServices
{
    public interface ICustomerService
    {
        Task<List<CustomerResponse>> GetAllAsync();
    }
}
