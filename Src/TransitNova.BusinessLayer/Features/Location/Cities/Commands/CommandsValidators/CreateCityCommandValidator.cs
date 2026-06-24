using FluentValidation;
using TransitNova.BusinessLayer.DTOs.City;
using TransitNova.BusinessLayer.Features.Location.Cities.Commands;
using TransitNova.BusinessLayer.Interfaces.Repositories.LocationRepository;

namespace TransitNova.BusinessLayer.Features.Location.Cities.Commands.CommandsValidators
{
    public sealed class CreateCityCommandValidator : AbstractValidator<CreateCityCommand>
    {
        public CreateCityCommandValidator(
            IValidator<CreateCityDto> dtoValidator,
            ICityRepository cityRepository,
            ICountryRepository countryRepository)
        {
            RuleFor(x => x.Dto)
                .NotNull()
                .SetValidator(dtoValidator);

            RuleFor(x => x.Dto.GovernmentId)
                .MustAsync(countryRepository.ExistsAsync)
                .WithMessage("Government not found.");

            RuleFor(x => x.Dto)
                .MustAsync(async (dto, ct) =>
                    !await cityRepository.NameExistsForAnotherGovernmentAsync(dto.GovernmentId, dto.Name, ct))
                .WithMessage("City name already exists in this government.");
        }
    }
}
