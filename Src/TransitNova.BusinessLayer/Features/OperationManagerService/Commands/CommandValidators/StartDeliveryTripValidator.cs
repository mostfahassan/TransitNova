using FluentValidation;
using TransitNova.BusinessLayer.Features.OperationManagerService.Commands.Trips;
using TransitNova.Domain.Enums.Result;
namespace TransitNova.BusinessLayer.Features.OperationManagerService.Commands.CommandValidators
{
    public sealed class StartDeliveryTripValidator : AbstractValidator<StartDeliveryTripCommand>
    {
        public StartDeliveryTripValidator()
        
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
