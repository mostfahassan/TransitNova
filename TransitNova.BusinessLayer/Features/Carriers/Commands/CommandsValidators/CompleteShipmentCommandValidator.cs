using FluentValidation;
using TransitNova.BusinessLayer.Interfaces.Repositories.CarrierRepository;
using TransitNova.BusinessLayer.Interfaces.Repositories.ShipmentRepository;
using TransitNova.Domain.Entities.MainEntities;
using TransitNova.Domain.Enums.Result;

namespace TransitNova.BusinessLayer.Features.Carriers.Commands.CommandsValidators
{
    public sealed class CompleteShipmentCommandValidator : AbstractValidator<CompleteShipmentToWarehouseCommand>
    {
        public CompleteShipmentCommandValidator(
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
