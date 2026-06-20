using FluentValidation;
using TransitNova.BusinessLayer.DTOs.Vehicle;

namespace TransitNova.BusinessLayer.Validators.VehicleValidators
{
    public class VehicleDtoValidator : AbstractValidator<VehicleDto>
    {
        public VehicleDtoValidator()
        {
            RuleFor(v => v.VehicleType)
                .IsInEnum()
                .WithMessage("Vehicle type is invalid.");

            RuleFor(v => v.PlateNumber)
                .NotEmpty()
                .WithMessage("Plate number is required.")
                .MaximumLength(50)
                .WithMessage("Plate number must not exceed 50 characters.");

            RuleFor(v => v.CapacityWeight)
                .GreaterThan(0)
                .WithMessage("Capacity weight must be greater than zero.");

            RuleFor(v => v.CapacityVolume)
                .GreaterThan(0)
                .WithMessage("Capacity volume must be greater than zero.");

            RuleFor(v => v.CarrierId)
                .NotEmpty()
                .WithMessage("Carrier id is required.");
        }
    }
}
