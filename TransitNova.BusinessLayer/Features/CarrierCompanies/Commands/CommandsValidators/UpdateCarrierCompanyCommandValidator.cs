using System.Security.Claims;
using FluentValidation;
using TransitNova.BusinessLayer.DTOs.CarrierCompany;
using TransitNova.BusinessLayer.Interfaces.Repositories.GenericRepository;
using TransitNova.BusinessLayer.Interfaces.Repositories.OperationManagerRepository;
using TransitNova.Domain.Entities.MainEntities;

namespace TransitNova.BusinessLayer.Features.CarrierCompanies.Commands.CommandsValidators
{
    public sealed class UpdateCarrierCompanyCommandValidator : AbstractValidator<UpdateCarrierCompanyCommand>
    {
        public UpdateCarrierCompanyCommandValidator(
            IValidator<UpdateCarrierCompany> dtoValidator,
            IOperationManagerRulesRepository operationManager,
            IGenericRepository<CarrierCompany, Guid> repository)
        {
            RuleFor(x => x.Dto)
                .NotNull()
                .SetValidator(dtoValidator);

            RuleFor(x => x.AdminId)
                .MustAsync(operationManager.ExistsAsync)
                .WithMessage("User not authenticated.");

            RuleFor(x => x.Id)
                .MustAsync(repository.ExistsAsync)
                .WithMessage("CarrierCompany not found.");
        }
    }
}
