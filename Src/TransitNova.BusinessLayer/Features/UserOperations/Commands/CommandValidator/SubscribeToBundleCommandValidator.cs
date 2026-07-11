using FluentValidation;
using TransitNova.BusinessLayer.Features.UserOperations.Commands.Bundles;
using TransitNova.Domain.Enums.Result;

namespace TransitNova.BusinessLayer.Features.UserOperations.Commands.CommandValidator
{
    public sealed class SubscribeToBundleCommandValidator : AbstractValidator<SubscribeToBundleCommand>
    {
        public SubscribeToBundleCommandValidator()
        {
            RuleFor(x => x.UserId)
                  .NotEmpty()
                  .WithErrorCode($"{ErrorCode.UNAUTHORIZED}");

            RuleFor(x => x.BundleId)
                .NotEmpty()
                .WithMessage($"{ErrorCode.REQUIRED_FIELD}");

            RuleFor(x => x.Dto)
                .NotNull()
                .WithMessage($"{ErrorCode.REQUIRED_FIELD}");

            RuleFor(x => x.Dto.PaymentMethod)
                .IsInEnum()
                .When(x => x.Dto is not null)
                .WithErrorCode($"{ErrorCode.REQUIRED_FIELD}");
        }
    }
}
