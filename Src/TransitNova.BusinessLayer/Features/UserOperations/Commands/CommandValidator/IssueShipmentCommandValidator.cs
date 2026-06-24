using FluentValidation;
using TransitNova.BusinessLayer.Interfaces.Repositories.ShipmentRepository;
using TransitNova.BusinessLayer.Interfaces.Repositories.UserRepository;
using TransitNova.Domain.Enums.Result;
using TransitNova.Domain.Enums.Shipment;
namespace TransitNova.BusinessLayer.Features.UserOperations.Commands.CommandValidator
{
    public sealed class IssueShipmentCommandValidator : AbstractValidator<IssueShipmentCommand>
    {
        private static readonly ShipmentStatuses[] _cancellableStates =
            [ShipmentStatuses.Pending, ShipmentStatuses.OnHold, ShipmentStatuses.Approved];

        public IssueShipmentCommandValidator(IShipmentRulesRepository shipmentRepository,IUserRulesRepository userRulesRepository)
        {
            RuleFor(x => x).CustomAsync(async (command, context, ct) =>
            {
               var cancellable = await shipmentRepository.IsEditableAsync(command.ShipmentId, _cancellableStates, ct);

                if (!cancellable)
                {
                    context.AddFailure($"Issue   failed: Shipment with Id {command.ShipmentId} is not in a cancellable state");
                    return;
                }
            });

            RuleFor(x => x.AppUserId)
                      .NotEmpty()
                      .WithErrorCode($"{ErrorCode.UNAUTHORIZED}");


        }
    }
}
