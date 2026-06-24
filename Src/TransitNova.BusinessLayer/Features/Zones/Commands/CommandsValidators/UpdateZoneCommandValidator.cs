using FluentValidation;
using TransitNova.BusinessLayer.DTOs.ZoneDtos;
using TransitNova.BusinessLayer.Interfaces.Repositories.LocationRepository;

namespace TransitNova.BusinessLayer.Features.Zones.Commands.CommandsValidators
{
    public sealed class UpdateZoneCommandValidator : AbstractValidator<UpdateZoneCommand>
    {
        public UpdateZoneCommandValidator(
            IValidator<UpdateZoneDto> dtoValidator,
            IZoneRepository zoneRepository)
        {
            RuleFor(x => x.Dto)
                .NotNull()
                .SetValidator(dtoValidator);

            RuleFor(x => x.Dto.ZoneId)
                .MustAsync(zoneRepository.ExistsAsync)
                .WithMessage("Zone not found.");

            RuleFor(x => x.Dto.CityId)
                .MustAsync(zoneRepository.CityExistsAsync)
                .WithMessage("City not found.");

            RuleFor(x => x.Dto)
                .MustAsync(async (dto, ct) =>
                    !await zoneRepository.NameExistsInCityForAnotherAsync(dto.ZoneId, dto.CityId, dto.Name, ct))
                .WithMessage("Zone name already exists in this city.");

            RuleFor(x => x.Dto)
                .MustAsync(async (dto, ct) =>
                    !await zoneRepository.CodeExistsInCityForAnotherAsync(dto.ZoneId, dto.CityId, dto.Code, ct))
                .WithMessage("Zone code already exists in this city.");
        }
    }
}
