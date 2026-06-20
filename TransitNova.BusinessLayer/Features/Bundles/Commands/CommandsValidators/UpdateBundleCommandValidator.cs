
using FluentValidation;
using TransitNova.BusinessLayer.DTOs.Bundle;
using TransitNova.BusinessLayer.Interfaces.Repositories.GenericRepository;
using TransitNova.BusinessLayer.Interfaces.Repositories.OperationManagerRepository;
using TransitNova.Domain.Entities.MainEntities;

namespace TransitNova.BusinessLayer.Features.Bundles.Commands.CommandsValidators
{
    public sealed class UpdateBundleCommandValidator : AbstractValidator<UpdateBundleCommand>
    {
        public UpdateBundleCommandValidator(
            IValidator<UpdateBundleDto> dtoValidator,
            IOperationManagerRulesRepository operationManagerRepository,
            IGenericRepository<Bundle, int> repository)
        {
            RuleFor(x => x.Dto)
                .NotNull()
                .SetValidator(dtoValidator);

            RuleFor(x => x.AppUserId)
                 .MustAsync(operationManagerRepository.ExistsAsync)
                 .WithMessage("Operation Manager Not Found");

            RuleFor(x => x.Dto.BundleId)
                .MustAsync(repository.ExistsAsync)
                .WithMessage("Bundle not found.");
        }
    }
}
