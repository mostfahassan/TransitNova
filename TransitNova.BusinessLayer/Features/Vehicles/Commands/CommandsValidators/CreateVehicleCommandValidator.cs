using FluentValidation;
using TransitNova.BusinessLayer.DTOs.Vehicle;
using TransitNova.BusinessLayer.Interfaces.Repositories.CarrierRepository;
using TransitNova.BusinessLayer.Interfaces.Repositories.VehicleRepository;
using TransitNova.Domain.Enums.Result;

namespace TransitNova.BusinessLayer.Features.Vehicles.Commands.CommandsValidators
{
    public sealed class CreateVehicleCommandValidator : AbstractValidator<CreateVehicleCommand>
    {
        public CreateVehicleCommandValidator(
            IValidator<VehicleDto> dtoValidator,
            IVehicleRulesRepository vehicleRepository,
            ICarrierRulesRepository carrierRepository)
        {
            RuleFor(x => x.Dto)
                .NotNull()
                .SetValidator(dtoValidator);

            RuleFor(x => x.Dto.CarrierId)
                .NotEmpty()
                .WithErrorCode($"{ErrorCode.UNAUTHORIZED}");

            RuleFor(x => x.Dto.PlateNumber)
                .MustAsync(async (plateNumber, ct) =>
                    !await vehicleRepository.ExistsByPlateNumberAsync(plateNumber.Trim(), ct))
                .WithMessage("Vehicle plate number already exists.");

            RuleFor(x => x.Dto.CarrierId)
                .MustAsync(async (carrierId, ct) =>
                    !await vehicleRepository.CarrierHasVehicleAsync(carrierId, ct))
                .WithMessage("Carrier already has a vehicle.");
        }
    }
}
