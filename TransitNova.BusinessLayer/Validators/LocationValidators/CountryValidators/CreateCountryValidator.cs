using FluentValidation;
using TransitNova.BusinessLayer.DTOs.Country;

namespace TransitNova.BusinessLayer.Validators.LocationValidators.CountryValidators
{
    public class CreateCountryValidator : AbstractValidator<CreateCountryDto>
    {
        public CreateCountryValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Country name is required.")
                .MaximumLength(100).WithMessage("Country name must not exceed 100 characters.");
        }
    }
}

