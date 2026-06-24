using FluentValidation;
using TransitNova.BusinessLayer.DTOs.Vehicle;
using TransitNova.BusinessLayer.Interfaces.Repositories.VehicleRepository;
using TransitNova.Domain.Enums.Result;
namespace TransitNova.BusinessLayer.Features.Vehicles.Commands.CommandsValidators
{
    public sealed class UpdateVehicleCommandValidator : AbstractValidator<UpdateVehicleCommand>
    {
        public UpdateVehicleCommandValidator(
            IValidator<UpdateVehicleDto> dtoValidator,
            IVehicleRulesRepository vehicleRepository) 
        
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Vehicle id is required.");

            RuleFor(x => x.Dto)
                .NotNull()
                .SetValidator(dtoValidator);

            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Vehicle Id Can't Be Empty");

            RuleFor(x => x.Dto.CarrierId)
                .NotEmpty()
                .WithErrorCode($"{ErrorCode.UNAUTHORIZED}");

            RuleFor(x => x)
                .MustAsync(async (command, ct) =>
                    !await vehicleRepository.PlateNumberExistsForAnotherVehicleAsync(
                        command.Id,
                        command.Dto.PlateNumber.Trim(),
                        ct))
                .WithMessage("Vehicle plate number already exists.");

            RuleFor(x => x)
                .MustAsync(async (command, ct) =>
                    !await vehicleRepository.CarrierHasAnotherVehicleAsync(command.Dto.CarrierId, command.Id, ct))
                .WithMessage("Carrier already has another vehicle.");
        }
    }
}
