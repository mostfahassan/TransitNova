using FluentValidation;
using TransitNova.BusinessLayer.DTOs.Country;
using TransitNova.BusinessLayer.Features.Location.Countries.Commands;
using TransitNova.BusinessLayer.Interfaces.Repositories.LocationRepository;

namespace TransitNova.BusinessLayer.Features.Location.Countries.Commands.CommandsValidators
{
    public sealed class UpdateCountryCommandValidator : AbstractValidator<UpdateCountryCommand>
    {
        public UpdateCountryCommandValidator(
            IValidator<UpdateCountryDto> dtoValidator,
            ICountryRepository countryRepository)
        {
            RuleFor(x => x.Dto)
                .NotNull()
                .SetValidator(dtoValidator);

            RuleFor(x => x.Dto.CountryId)
                .MustAsync(countryRepository.ExistsAsync)
                .WithMessage("Country not found.");

            RuleFor(x => x.Dto)
                .MustAsync(async (dto, ct) =>
                    !await countryRepository.NameExistsForAnotherAsync(dto.CountryId, dto.Name, ct))
                .WithMessage("Country name already exists.");
        }
    }
}
