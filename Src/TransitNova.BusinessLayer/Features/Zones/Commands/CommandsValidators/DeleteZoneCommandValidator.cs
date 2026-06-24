using FluentValidation;
using TransitNova.BusinessLayer.Interfaces.Repositories.LocationRepository;

namespace TransitNova.BusinessLayer.Features.Zones.Commands.CommandsValidators
{
    public sealed class DeleteZoneCommandValidator : AbstractValidator<DeleteZoneCommand>
    {
        public DeleteZoneCommandValidator(IZoneRepository zoneRepository)
        {
            RuleFor(x => x.Id)
                .MustAsync(zoneRepository.ExistsAsync)
                .WithMessage("Zone not found.");
        }
    }
}
