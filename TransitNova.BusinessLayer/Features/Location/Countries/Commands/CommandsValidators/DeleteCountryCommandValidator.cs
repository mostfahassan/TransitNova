using FluentValidation;
using TransitNova.BusinessLayer.Features.Location.Countries.Commands;
using TransitNova.BusinessLayer.Interfaces.Repositories.LocationRepository;

namespace TransitNova.BusinessLayer.Features.Location.Countries.Commands.CommandsValidators
{
    public sealed class DeleteCountryCommandValidator : AbstractValidator<DeleteCountryCommand>
    {
        public DeleteCountryCommandValidator(ICountryRepository countryRepository)
        {
            RuleFor(x => x.Id)
                .MustAsync(countryRepository.ExistsAsync)
                .WithMessage("Country not found.");
        }
    }
}
