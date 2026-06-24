using FluentValidation;
using TransitNova.BusinessLayer.Features.UserOperations.Commands;
using TransitNova.BusinessLayer.Interfaces.Repositories.UserRepository;
using TransitNova.Domain.Enums.Result;

namespace TransitNova.BusinessLayer.Features.UserOperations.Commands.CommandValidator
{
    public sealed class UnsubscribeFromBundleCommandValidator : AbstractValidator<UnsubscribeFromBundleCommand>
    {
        public UnsubscribeFromBundleCommandValidator()
        {
            RuleFor(x => x.UserId)
                  .NotEmpty()
                  .WithErrorCode($"{ErrorCode.UNAUTHORIZED}");


            RuleFor(x => x.BundleId)
             .NotEmpty()
             .WithMessage($"{ErrorCode.REQUIRED_FIELD}");
        }
    }
}
