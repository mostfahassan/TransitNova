using FluentValidation;
using TransitNova.BusinessLayer.DTOs.Country;

namespace TransitNova.BusinessLayer.Validators.LocationValidators.CountryValidators
{
    public class UpdateCountryValidator : AbstractValidator<UpdateCountryDto>
    {
        public UpdateCountryValidator()
        {
            RuleFor(x => x.CountryId).GreaterThan(0).WithMessage("CountryId is required.");
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Country name is required.")
                .MaximumLength(100).WithMessage("Country name must not exceed 100 characters.");
        }
    }
}

