using FluentValidation;
using TransitNova.BusinessLayer.Interfaces.Repositories.CarrierRepository;
using TransitNova.Domain.Enums.Result;

namespace TransitNova.BusinessLayer.Features.Carriers.Commands.CommandsValidators
{
    public sealed class CarrierAvailabilityCommandValidator : AbstractValidator<CarrierAvailabilityCommand>
    {
        public  CarrierAvailabilityCommandValidator(ICarrierRulesRepository carrierRepository)
        {
            RuleFor(x => x.CarrierId)
               .NotEmpty()
               .WithErrorCode($"{ErrorCode.UNAUTHORIZED}");
        }
    }
}
