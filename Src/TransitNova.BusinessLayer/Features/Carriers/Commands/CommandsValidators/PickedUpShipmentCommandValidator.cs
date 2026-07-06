using FluentValidation;
using TransitNova.BusinessLayer.Interfaces.Repositories.CarrierRepository;
using TransitNova.BusinessLayer.Interfaces.Repositories.ShipmentRepository;
using TransitNova.Domain.Enums.Result;

namespace TransitNova.BusinessLayer.Features.Carriers.Commands.CommandsValidators
{
    public sealed class PickedUpShipmentCommandValidator : AbstractValidator<PickedUpShipmentCommand>
    {
        public PickedUpShipmentCommandValidator(
            ICarrierRulesRepository carrierRepository,
            IShipmentRulesRepository shipmentRepository)
        {
            RuleFor(x => x.CarrierId)
                .NotEmpty()
                .WithErrorCode($"{ErrorCode.UNAUTHORIZED}");

            RuleFor(x => x.ShipmentId)
               .NotEmpty()
               .WithErrorCode($"{ErrorCode.REQUIRED_FIELD}");
        }
    }
}
