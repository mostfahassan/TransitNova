using FluentValidation;
using TransitNova.BusinessLayer.Interfaces.Repositories.AdminRepository;
using TransitNova.BusinessLayer.Interfaces.Repositories.CarrierRepository;
using TransitNova.Domain.Enums.Result;
namespace TransitNova.BusinessLayer.Features.Carriers.Commands.CommandsValidators
{
    public sealed class DeleteCarrierCommandValidator : AbstractValidator<DeleteCarrierCommand>
    {
        public DeleteCarrierCommandValidator(ICarrierRulesRepository carrierRepository,IAdminRulesRepository admin)
        {
            RuleFor(x => x.CarrierId)
               .NotEmpty()
                .WithErrorCode($"{ErrorCode.UNAUTHORIZED}");
            RuleFor(x => x.AdminId)
                .MustAsync(admin.IsAdminExistsAsync)
                .WithMessage(command => $" admin With AmdminId => {command.AdminId} Not Found");
        }
    }
}
