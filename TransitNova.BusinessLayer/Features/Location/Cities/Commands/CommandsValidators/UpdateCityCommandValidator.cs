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

            RuleFor(x => x.Dto.Id)
                .MustAsync(cityRepository.ExistsAsync)
                .WithMessage("City not found.");

            RuleFor(x => x.Dto.GovernmentId)
                .MustAsync(countryRepository.ExistsAsync)
                .WithMessage("Government not found.");

            RuleFor(x => x.Dto)
                .MustAsync(async (dto, ct) =>
                    !await cityRepository.NameExistsForAnotherAsync(dto.Id,dto.Name, ct, dto.GovernmentId))
                .WithMessage("City name already exists in this country.");
        }
    }
}
