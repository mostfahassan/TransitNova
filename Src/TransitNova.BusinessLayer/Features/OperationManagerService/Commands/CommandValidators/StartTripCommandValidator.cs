using FluentValidation;
using TransitNova.BusinessLayer.Features.OperationManagerService.Commands.Trips;
using TransitNova.Domain.Enums.Result;
namespace TransitNova.BusinessLayer.Features.OperationManagerService.Commands.CommandValidators
{
    public sealed class StartTripCommandValidator : AbstractValidator<StartPickupTripCommand>
    {
        public StartTripCommandValidator()
        {
            RuleFor(x => x.OperationManagerId)
           .NotEmpty()
           .WithErrorCode($"{ErrorCode.UNAUTHORIZED}");

            RuleFor(x => x.CarrierId)
                .NotEmpty()
              .WithErrorCode($"{ErrorCode.UNAUTHORIZED}");
        }
    }
}
