using FluentValidation;
using TransitNova.BusinessLayer.DTOs.ZoneDtos;
using TransitNova.BusinessLayer.Interfaces.Repositories.LocationRepository;

namespace TransitNova.BusinessLayer.Features.Zones.Commands.CommandsValidators
{
    public sealed class CreateZoneCommandValidator : AbstractValidator<CreateZoneCommand>
    {
        public CreateZoneCommandValidator(
            IValidator<CreateZoneDto> dtoValidator,
            IZoneRepository zoneRepository)
        {
            RuleFor(x => x.Dto)
                .NotNull()
                .SetValidator(dtoValidator);

            RuleFor(x => x.Dto.CityId)
                .MustAsync(zoneRepository.CityExistsAsync)
                .WithMessage("City not found.");

            RuleFor(x => x.Dto)
                .MustAsync(async (dto, ct) =>
                    !await zoneRepository.NameExistsInCityAsync(dto.CityId, dto.Name, ct))
                .WithMessage("Zone name already exists in this city.");

            RuleFor(x => x.Dto)
                .MustAsync(async (dto, ct) =>
                    !await zoneRepository.CodeExistsInCityAsync(dto.CityId, dto.Code, ct))
                .WithMessage("Zone code already exists in this city.");
        }
    }
}
