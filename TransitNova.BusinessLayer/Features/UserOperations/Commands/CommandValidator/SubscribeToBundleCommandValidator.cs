using FluentValidation;
using TransitNova.BusinessLayer.Features.UserOperations.Commands;
using TransitNova.BusinessLayer.Interfaces.Repositories.UserRepository;
using TransitNova.Domain.Enums.Result;

namespace TransitNova.BusinessLayer.Features.UserOperations.Commands.CommandValidator
{
    public sealed class SubscribeToBundleCommandValidator : AbstractValidator<SubscribeToBundleCommand>
    {
        public SubscribeToBundleCommandValidator(IUserQueryRepository userRepository)
        {
            RuleFor(x => x.UserId)
                  .NotEmpty()
                  .WithErrorCode($"{ErrorCode.UNAUTHORIZED}");


            RuleFor(x => x.BundleId)
                .GreaterThan(0)
                .WithMessage("Bundle ID must be greater than 0.");
        }
    }
}
