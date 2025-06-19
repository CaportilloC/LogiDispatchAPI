using Application.DTOs.Orders;
using Application.Wrappers.Common;
using MediatR;

namespace Application.Features.Orders.Queries.GetById
{
    public class GetOrderByIdQuery : IRequest<BaseWrapperResponse<OrderResponse>>
    {
        public Guid Id { get; set; }

        public GetOrderByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}
