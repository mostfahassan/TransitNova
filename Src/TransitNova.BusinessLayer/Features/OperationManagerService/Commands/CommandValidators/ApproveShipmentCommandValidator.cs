using FluentValidation;
using TransitNova.BusinessLayer.Features.OperationManagerService.Commands.Shipments;
using TransitNova.Domain.Enums.Result;
namespace TransitNova.BusinessLayer.Features.OperationManagerService.Commands.CommandValidators
{
    public sealed class ApproveShipmentCommandValidator : AbstractValidator<ApproveShipmentCommand>
    {
        public ApproveShipmentCommandValidator()

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
