using Application.DTOs.Orders;
using Application.Wrappers;
using MediatR;

namespace Application.Features.Orders.Queries.GetById
{
    public class GetOrderByIdQuery : IRequest<WrapperResponse<OrderResponse>>
    {
        public Guid Id { get; set; }

        public GetOrderByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}
