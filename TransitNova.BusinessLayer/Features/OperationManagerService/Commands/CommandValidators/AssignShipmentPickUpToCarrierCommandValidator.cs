using FluentValidation;
using TransitNova.BusinessLayer.Features.OperationManagerService.Commands;
using TransitNova.BusinessLayer.Interfaces.Repositories.OperationManagerRepository;
using TransitNova.BusinessLayer.Interfaces.Repositories.ShipmentRepository;
namespace TransitNova.BusinessLayer.Features.OperationManagerService.Commands.CommandValidators
{
    public sealed class AssignShipmentPickUpToCarrierCommandValidator : AbstractValidator<AssignShipmentPickUpToCarrierCommand>
    {
        public AssignShipmentPickUpToCarrierCommandValidator(
            IOperationManagerRulesRepository managerRepository,
            IShipmentRulesRepository shipmentRepository)

        {
            RuleFor(x => x.OperationManagerId)
                .MustAsync(managerRepository.ExistsAsync)
                .WithMessage("Operation manager not found.");

            RuleFor(x => x.CarrierId)
            .NotEmpty()
            .WithMessage("Carrier Id Cant Be Empty");

            RuleFor(x => x.ShipmentId)
               .NotEmpty()
               .WithMessage("Shipment Id Cant Be Empty.");
        }
    }
}
