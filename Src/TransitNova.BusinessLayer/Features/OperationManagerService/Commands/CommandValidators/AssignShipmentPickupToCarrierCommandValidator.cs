using FluentValidation;
using TransitNova.BusinessLayer.Features.OperationManagerService.Commands.Carriers;
using TransitNova.BusinessLayer.Interfaces.Repositories.OperationManagerRepository;
namespace TransitNova.BusinessLayer.Features.OperationManagerService.Commands.CommandValidators
{
    public sealed class AssignShipmentPickupToCarrierCommandValidator : AbstractValidator<AssignShipmentPickupToCarrierCommand>
    {
        public AssignShipmentPickupToCarrierCommandValidator(IOperationManagerRulesRepository managerRepository)
        {
            RuleFor(x => x.OperationManagerId)
                .MustAsync(managerRepository.ExistsAsync)
                .WithMessage("Operation manager not found.");

            RuleFor(x => x.CarrierId)
            .NotEmpty()
            .WithMessage("Carrier ID cannot be empty.");

            RuleFor(x => x.ShipmentId)
               .NotEmpty()
               .WithMessage("Shipment ID cannot be empty.");
        }
    }
}
