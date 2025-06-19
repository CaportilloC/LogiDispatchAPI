using Application.Utils;
using FluentValidation;

namespace Application.Features.Orders.Commands.Restore
{
    public class RestoreOrderCommandValidator : AbstractValidator<RestoreOrderCommand>
    {
        public RestoreOrderCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage(Constants.RequiredField);
        }
    }
}
