using FluentValidation;
using TransitNova.BusinessLayer.DTOs.Country;
using TransitNova.BusinessLayer.Features.Location.Countries.Commands;
using TransitNova.BusinessLayer.Interfaces.Repositories.LocationRepository;

namespace TransitNova.BusinessLayer.Features.Location.Countries.Commands.CommandsValidators
{
    public sealed class CreateCountryCommandValidator : AbstractValidator<CreateCountryCommand>
    {
        public CreateCountryCommandValidator(
            IValidator<CreateCountryDto> dtoValidator,
            ICountryRepository countryRepository)
        {
            RuleFor(x => x.Dto)
                .NotNull()
                .SetValidator(dtoValidator);

            RuleFor(x => x.Dto.Name)
                .MustAsync(async (name, ct) => !await countryRepository.NameExistsAsync(name, ct))
                .WithMessage("Country name already exists.");
        }
    }
}
