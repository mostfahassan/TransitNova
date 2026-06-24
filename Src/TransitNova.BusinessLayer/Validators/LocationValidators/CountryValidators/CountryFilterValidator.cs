using FluentValidation;
using TransitNova.BusinessLayer.DTOs.Country;

namespace TransitNova.BusinessLayer.Validators.LocationValidators.CountryValidators
{
    public class CountryFilterValidator : AbstractValidator<CountryFilterDto>
    {
        public CountryFilterValidator()
        {
            RuleFor(x => x.PageNumber).GreaterThanOrEqualTo(1).WithMessage("Page number must be at least 1.");
            RuleFor(x => x.PageSize).InclusiveBetween(1, 100).WithMessage("Page size must be between 1 and 100.");
            RuleFor(x => x.SearchTerm)
                .MaximumLength(100)
                .When(x => !string.IsNullOrWhiteSpace(x.SearchTerm))
                .WithMessage("Search term cannot exceed 100 characters.");
        }
    }
}

