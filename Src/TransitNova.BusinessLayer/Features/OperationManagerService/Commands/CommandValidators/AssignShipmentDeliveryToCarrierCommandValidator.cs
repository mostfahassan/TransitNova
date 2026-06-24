using FluentValidation;
using TransitNova.BusinessLayer.Features.OperationManagerService.Commands.Carriers;
using TransitNova.BusinessLayer.Interfaces.Repositories.CarrierRepository;
using TransitNova.BusinessLayer.Interfaces.Repositories.OperationManagerRepository;
using TransitNova.BusinessLayer.Interfaces.Repositories.ShipmentRepository;
using TransitNova.Domain.Entities.MainEntities;
using TransitNova.Domain.Enums.Result;
using TransitNova.Domain.Enums.Shipment;

namespace TransitNova.BusinessLayer.Features.OperationManagerService.Commands.CommandValidators
{
    public sealed class AssignShipmentDeliveryToCarrierCommandValidator : AbstractValidator<AssignShipmentDeliveryToCarrierCommand>
    {
        public AssignShipmentDeliveryToCarrierCommandValidator(
            IOperationManagerRulesRepository managerRepository,
            ICarrierRulesRepository carrierRulesRepository,
            IShipmentRulesRepository shipmentRepository)

        {
            RuleFor(x => x.OperationManagerId)
                .NotEmpty()
                .WithErrorCode($"{ErrorCode.UNAUTHORIZED}");

            RuleFor(x => x.CarrierId)
                .NotEmpty()
                .WithErrorCode($"{ErrorCode.UNAUTHORIZED}");

            RuleFor(x => x.ShipmentId)
                .NotEmpty()
                .WithMessage("Shipment Id Cant Be Empty.");

        }
    }
}
