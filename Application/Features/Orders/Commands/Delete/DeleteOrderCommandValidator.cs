using Application.Utils;
using FluentValidation;

namespace Application.Features.Orders.Commands.Delete
{
    public class DeleteOrderCommandValidator : AbstractValidator<DeleteOrderCommand>
    {
        public DeleteOrderCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage(Constants.RequiredField);
        }
    }
}
