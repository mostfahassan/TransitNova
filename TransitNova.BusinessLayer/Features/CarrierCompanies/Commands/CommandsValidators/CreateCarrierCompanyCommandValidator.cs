using System.Security.Claims;
using FluentValidation;
using TransitNova.BusinessLayer.DTOs.CarrierCompany;
using TransitNova.BusinessLayer.Interfaces.Repositories.OperationManagerRepository;

namespace TransitNova.BusinessLayer.Features.CarrierCompanies.Commands.CommandsValidators
{
    public sealed class CreateCarrierCompanyCommandValidator : AbstractValidator<CreateCarrierCompanyCommand>
    {
        public CreateCarrierCompanyCommandValidator(IValidator<AddCarrierCompany> dtoValidator ,IOperationManagerRulesRepository operationManager)
        {
            RuleFor(x => x.Dto)
                .NotNull()
                .SetValidator(dtoValidator);

            RuleFor(x => x.AdminId)
                .MustAsync(operationManager.ExistsAsync)
                .WithMessage("User not authenticated.");
        }
    }
}
