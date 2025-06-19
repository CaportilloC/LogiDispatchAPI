using Application.Wrappers;
using MediatR;

namespace Application.Features.Orders.Commands.Delete
{
    public class DeleteOrderCommand : IRequest<WrapperResponse<bool>>
    {
        public Guid Id { get; set; }

        public DeleteOrderCommand(Guid id)
        {
            Id = id;
        }
    }
}
