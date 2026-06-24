using FluentValidation;
using TransitNova.BusinessLayer.DTOs.City;

namespace TransitNova.BusinessLayer.Validators.LocationValidators.CityValidators
{
    public class UpdateCityValidator : AbstractValidator<UpdateCityDto>
    {
        public UpdateCityValidator()
        {
            RuleFor(x => x.GovernmentId).GreaterThan(0).WithMessage("GovernmentId is required.");
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("City name is required.")
                .MaximumLength(100).WithMessage("City name must not exceed 100 characters.");
        }
    }
}
