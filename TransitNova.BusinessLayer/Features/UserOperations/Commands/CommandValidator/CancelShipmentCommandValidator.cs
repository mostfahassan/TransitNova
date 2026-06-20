using FluentValidation;
using TransitNova.BusinessLayer.Interfaces.Repositories.ShipmentRepository;
using TransitNova.BusinessLayer.Interfaces.Repositories.UserRepository;
using TransitNova.Domain.Enums.Result;
using TransitNova.Domain.Enums.Shipment;

namespace TransitNova.BusinessLayer.Features.UserOperations.Commands.CommandValidator
{
    public sealed class CancelShipmentCommandValidator : AbstractValidator<CancelShipmentCommand>
    {
        private static readonly ShipmentStatuses[] CancellableStates =
            [ShipmentStatuses.Pending, ShipmentStatuses.OnHold, ShipmentStatuses.Approved];

        public CancelShipmentCommandValidator(IShipmentRulesRepository shipmentRepository,IUserRulesRepository user)
        {
            RuleFor(x => x).CustomAsync(async (command, context, ct) =>
            {
               var cancellable = await shipmentRepository.Editable(command.ShipmentId, CancellableStates, ct);

                if (!cancellable)
                {
                    context.AddFailure($"Cancel failed: Shipment with ID {command.ShipmentId} is not in a cancellable state");
                    return;
                }
            });

            RuleFor(x => x.AppUserId)
                .NotEmpty()
                .WithErrorCode($"{ErrorCode.UNAUTHORIZED}");

            }

        }
    }

