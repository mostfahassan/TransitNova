using FluentValidation;
using TransitNova.BusinessLayer.DTOs.ZoneDtos;

namespace TransitNova.BusinessLayer.Validators.LocationValidators.ZoneValidators
{
    public class UpdateZoneValidator : AbstractValidator<UpdateZoneDto>
    {
        public UpdateZoneValidator()
        {
            RuleFor(x => x.ZoneId).NotEmpty().WithMessage("ZoneId is required.");
            RuleFor(x => x.CityId).GreaterThan(0).WithMessage("CityId is required.");
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Zone name is required.")
                .MaximumLength(100).WithMessage("Zone name must not exceed 100 characters.");
            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("Zone code is required.")
                .MaximumLength(50).WithMessage("Zone code must not exceed 50 characters.");
        }
    }
}

