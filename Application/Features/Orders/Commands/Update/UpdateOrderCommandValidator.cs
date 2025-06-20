using Application.Utils;
using FluentValidation;

namespace Application.Features.Orders.Commands.Update
{
    public class UpdateOrderCommandValidator : AbstractValidator<UpdateOrderCommand>
    {
        public UpdateOrderCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage(Constants.RequiredField);

            RuleFor(x => x.CustomerId)
                .NotEmpty().WithMessage(Constants.RequiredField);

            RuleFor(x => x.Items)
                .NotNull().WithMessage(Constants.RequiredField)
                .Must(items => items.Any()).WithMessage(Constants.MustHaveAtLeastOneItem);

            RuleFor(x => x.OriginLatitude)
                .InclusiveBetween(-90, 90).WithMessage(Constants.InvalidLatitude);

            RuleFor(x => x.OriginLongitude)
                .InclusiveBetween(-180, 180).WithMessage(Constants.InvalidLongitude);

            RuleFor(x => x.DestinationLatitude)
                .InclusiveBetween(-90, 90).WithMessage(Constants.InvalidLatitude);

            RuleFor(x => x.DestinationLongitude)
                .InclusiveBetween(-180, 180).WithMessage(Constants.InvalidLongitude);
        }
    }
}
