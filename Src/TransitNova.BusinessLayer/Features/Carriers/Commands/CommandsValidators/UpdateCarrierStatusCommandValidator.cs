using FluentValidation;
using TransitNova.BusinessLayer.Interfaces.Repositories.CarrierRepository;
using TransitNova.Domain.Enums.Result;
namespace TransitNova.BusinessLayer.Features.Carriers.Commands.CommandsValidators
{
    public sealed class UpdateCarrierStatusCommandValidator : AbstractValidator<UpdateCarrierStatusCommand>
    {
        public UpdateCarrierStatusCommandValidator(ICarrierRulesRepository carrierRepository)
        {
            RuleFor(x => x.Status)
                .Must(status => Enum.IsDefined(status))
                .WithMessage($"{ErrorCode.INVALID_STATE}");
 
        }
    }
}
