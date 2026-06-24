using FluentValidation;
using TransitNova.BusinessLayer.DTOs.City;
using TransitNova.BusinessLayer.Features.Location.Cities.Commands;
using TransitNova.BusinessLayer.Interfaces.Repositories.LocationRepository;

namespace TransitNova.BusinessLayer.Features.Location.Cities.Commands.CommandsValidators
{
    public sealed class UpdateCityCommandValidator : AbstractValidator<UpdateCityCommand>
    {
        public UpdateCityCommandValidator(
            IValidator<UpdateCityDto> dtoValidator,
            ICityRepository cityRepository,
            ICountryRepository countryRepository)
        {
            RuleFor(x => x.Dto)
                .NotNull()
                .SetValidator(dtoValidator);

            RuleFor(x => x.CityId)
                .GreaterThan(0)
                .WithMessage("CityId is required.")
                .MustAsync(cityRepository.ExistsAsync)
                .WithMessage("City not found.");

            RuleFor(x => x.Dto.GovernmentId)
                .MustAsync(countryRepository.ExistsAsync)
                .WithMessage("Government not found.");

            RuleFor(x => x)
                .MustAsync(async (command, ct) =>
                    !await cityRepository.NameExistsForAnotherAsync(command.CityId, command.Dto.Name, ct, command.Dto.GovernmentId))
                .WithMessage("City name already exists in this country.");
        }
    }
}
