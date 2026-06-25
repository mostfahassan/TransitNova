using FluentValidation;
using TransitNova.BusinessLayer.Features.OperationManagerService.Commands.Carriers;
using TransitNova.Domain.Enums.Result;
namespace TransitNova.BusinessLayer.Features.OperationManagerService.Commands.CommandValidators
{
    public sealed class AssignShipmentDeliveryToCarrierCommandValidator : AbstractValidator<AssignShipmentDeliveryToCarrierCommand>
    {
        public AssignShipmentDeliveryToCarrierCommandValidator()
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
