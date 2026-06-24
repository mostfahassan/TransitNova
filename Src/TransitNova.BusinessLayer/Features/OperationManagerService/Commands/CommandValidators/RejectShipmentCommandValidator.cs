using FluentValidation;
using TransitNova.BusinessLayer.Features.OperationManagerService.Commands.Shipments;
using TransitNova.BusinessLayer.Interfaces.Repositories.OperationManagerRepository;
using TransitNova.BusinessLayer.Interfaces.Repositories.ShipmentRepository;
using TransitNova.Domain.Enums.Result;
namespace TransitNova.BusinessLayer.Features.OperationManagerService.Commands.CommandValidators
{
    public sealed class RejectShipmentCommandValidator : AbstractValidator<RejectShipmentCommand>
    {
        public RejectShipmentCommandValidator(
            IOperationManagerRulesRepository managerRepository,
            IShipmentRulesRepository shipmentRepository)
        {
            RuleFor(x => x.OperationManagerId)
                .NotEmpty()
               .WithErrorCode($"{ErrorCode.UNAUTHORIZED}");

            RuleFor(x => x.ShipmentId)
                .NotEmpty()
                .WithMessage("Shipment Id Cant Be Empty.");
        }
    }
}
