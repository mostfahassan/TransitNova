using FluentValidation;
using TransitNova.BusinessLayer.DTOs.Shipment;
using TransitNova.BusinessLayer.Features.UserOperations.Commands;
using TransitNova.BusinessLayer.Interfaces.Repositories.ShipmentRepository;
using TransitNova.BusinessLayer.Interfaces.Repositories.UserRepository;
using TransitNova.Domain.Entities.MainEntities;
using TransitNova.Domain.Enums.Result;
using TransitNova.Domain.Enums.Shipment;

namespace TransitNova.BusinessLayer.Features.UserOperations.Commands.CommandValidator
{
    public sealed class UpdateShipmentCommandValidator : AbstractValidator<UpdateShipmentCommand>
    {
        private static readonly ShipmentStatuses[] UpdatableStates =
            [ShipmentStatuses.Pending, ShipmentStatuses.OnHold, ShipmentStatuses.Approved, ShipmentStatuses.Rejected];

        public UpdateShipmentCommandValidator(
            IValidator<UpdateShipmentDto> dtoValidator,
            IShipmentRulesRepository shipmentRepository,IUserRulesRepository userRulesRepository)
        {
            RuleFor(x => x.Dto)
                .NotNull()
                .SetValidator(dtoValidator);

            RuleFor(x => x).CustomAsync(async (command, context, ct) =>
            {
                var exists = await shipmentRepository.Editable(command.ShipmentId, UpdatableStates, ct);

                if (!exists)
                {
                    context.AddFailure($"Shipment With Id => {command.ShipmentId} Not Found");
                    return;
                }
            });

            RuleFor(x => x.AppUserId)
              .NotEmpty()
              .WithErrorCode($"{ErrorCode.UNAUTHORIZED}");


        }
    }
}
