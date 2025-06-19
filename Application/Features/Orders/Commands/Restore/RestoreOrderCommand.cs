using Application.Wrappers;
using MediatR;

namespace Application.Features.Orders.Commands.Restore
{
    public class RestoreOrderCommand : IRequest<WrapperResponse<bool>>
    {
        public Guid Id { get; set; }

        public RestoreOrderCommand(Guid id)
        {
            Id = id;
        }
    }
}
