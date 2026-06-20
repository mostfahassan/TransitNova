using FluentValidation;
using TransitNova.BusinessLayer.Interfaces.Repositories.GenericRepository;
using TransitNova.Domain.Entities.MainEntities;

namespace TransitNova.BusinessLayer.Features.CarrierCompanies.Commands.CommandsValidators
{
    public sealed class DeleteCarrierCompanyCommandValidator : AbstractValidator<DeleteCarrierCompanyCommand>
    {
        public DeleteCarrierCompanyCommandValidator(IGenericRepository<CarrierCompany, Guid> repository)
        {
            RuleFor(x => x.Id)
                .MustAsync(repository.ExistsAsync)
                .WithMessage("Carrier Company not found.");
        }
    }
}
