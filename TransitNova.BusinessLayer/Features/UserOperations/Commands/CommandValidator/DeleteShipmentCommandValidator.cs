using FluentValidation;
using TransitNova.BusinessLayer.Interfaces.Repositories.ShipmentRepository;
using TransitNova.BusinessLayer.Interfaces.Repositories.UserRepository;
using TransitNova.Domain.Enums.Result;
namespace TransitNova.BusinessLayer.Features.UserOperations.Commands.CommandValidator
{
    public sealed class DeleteShipmentCommandValidator : AbstractValidator<DeleteShipmentCommand>
    {
        public DeleteShipmentCommandValidator(IShipmentRulesRepository shipmentRepository, IUserRulesRepository userRulesRepository)
        {
            RuleFor(x => x.ShipmentId)
               .NotEmpty()
               .WithErrorCode($"{ErrorCode.SHIPMENT_NOT_FOUND}");

            RuleFor(x => x.AppUserId)
                   .NotEmpty()
                   .WithErrorCode($"{ErrorCode.UNAUTHORIZED}");
        }
    }
}
