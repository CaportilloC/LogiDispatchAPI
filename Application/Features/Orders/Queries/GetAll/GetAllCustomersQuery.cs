using Application.DTOs.Orders.Customers;
using Application.Wrappers;
using MediatR;

namespace Application.Features.Orders.Queries.GetAll
{
    public class GetAllCustomersQuery : IRequest<WrapperResponse<List<CustomerResponse>>>
    {
    }
}
