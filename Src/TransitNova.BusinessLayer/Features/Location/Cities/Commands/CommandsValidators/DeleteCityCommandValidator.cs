using FluentValidation;
using TransitNova.BusinessLayer.Features.Location.Cities.Commands;
using TransitNova.BusinessLayer.Interfaces.Repositories.LocationRepository;

namespace TransitNova.BusinessLayer.Features.Location.Cities.Commands.CommandsValidators
{
    public sealed class DeleteCityCommandValidator : AbstractValidator<DeleteCityCommand>
    {
        public DeleteCityCommandValidator(ICityRepository cityRepository)
        {
            RuleFor(x => x.Id)
                .MustAsync(cityRepository.ExistsAsync)
                .WithMessage("City not found.");
        }
    }
}
