using FluentValidation;
using TransitNova.BusinessLayer.DTOs.Vehicle;
using TransitNova.BusinessLayer.Interfaces.Repositories.CarrierRepository;
using TransitNova.BusinessLayer.Interfaces.Repositories.VehicleRepository;

namespace TransitNova.BusinessLayer.Features.Vehicles.Commands.CommandsValidators
{
    public sealed class CreateVehicleCommandValidator : AbstractValidator<CreateVehicleCommand>
    {
        public CreateVehicleCommandValidator(
            IValidator<CreateVehicleDto> dtoValidator,
            IVehicleRulesRepository vehicleRepository,
            ICarrierRulesRepository carrierRepository)
        {
            RuleFor(x => x.Dto)
                .NotNull()
                .SetValidator(dtoValidator);

            When(x => x.Dto is not null, () =>
            {
                RuleFor(x => x.Dto.PlateNumber)
                    .MustAsync(async (plateNumber, ct) =>
                        !await vehicleRepository.ExistsByPlateNumberAsync(plateNumber.Trim(), ct))
                    .When(x => !string.IsNullOrWhiteSpace(x.Dto.PlateNumber))
                    .WithMessage("Vehicle plate number already exists.");

                RuleFor(x => x.Dto.CarrierId)
                    .MustAsync(carrierRepository.IsCarrierExists)
                    .When(x => x.Dto.CarrierId != Guid.Empty)
                    .WithMessage("Carrier not found.");

                RuleFor(x => x.Dto.CarrierId)
                    .MustAsync(async (carrierId, ct) =>
                        !await vehicleRepository.CarrierHasVehicleAsync(carrierId, ct))
                    .When(x => x.Dto.CarrierId != Guid.Empty)
                    .WithMessage("Carrier already has a vehicle.");
            });
        }
    }
}
