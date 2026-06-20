using FluentValidation;
using TransitNova.BusinessLayer.Interfaces.Repositories.GenericRepository;
using TransitNova.Domain.Entities.MainEntities;

namespace TransitNova.BusinessLayer.Features.Bundles.Commands.CommandsValidators
{
    public sealed class DeleteBundleCommandValidator : AbstractValidator<DeleteBundleCommand>
    {
        public DeleteBundleCommandValidator(IGenericRepository<Bundle, int> repository)
        {
            RuleFor(x => x.Id)
                .MustAsync(repository.ExistsAsync)
                .WithMessage("Bundle not found.");
        }
    }
}
