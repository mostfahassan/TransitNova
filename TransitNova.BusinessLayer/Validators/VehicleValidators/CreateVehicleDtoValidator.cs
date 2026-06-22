using FluentValidation;
using TransitNova.BusinessLayer.DTOs.Vehicle;

namespace TransitNova.BusinessLayer.Validators.VehicleValidators
{
    public sealed class CreateVehicleDtoValidator : AbstractValidator<CreateVehicleDto>
    {
        public CreateVehicleDtoValidator()
        {
            RuleFor(vehicle => vehicle.VehicleType)
                .IsInEnum()
                .WithMessage("Vehicle type is invalid.");

            RuleFor(vehicle => vehicle.PlateNumber)
                .NotEmpty()
                .WithMessage("Plate number is required.")
                .MaximumLength(50)
                .WithMessage("Plate number must not exceed 50 characters.");

            RuleFor(vehicle => vehicle.CapacityWeight)
                .GreaterThan(0)
                .WithMessage("Capacity weight must be greater than zero.");

            RuleFor(vehicle => vehicle.CapacityVolume)
                .GreaterThan(0)
                .WithMessage("Capacity volume must be greater than zero.");

            RuleFor(vehicle => vehicle.CarrierId)
                .NotEmpty()
                .WithMessage("Carrier id is required.");
        }
    }
}
