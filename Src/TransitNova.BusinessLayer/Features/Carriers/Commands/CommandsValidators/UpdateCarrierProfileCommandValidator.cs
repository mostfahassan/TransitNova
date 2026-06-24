using FluentValidation;
using TransitNova.BusinessLayer.DTOs.Carrier;
using TransitNova.BusinessLayer.Interfaces.Repositories.CarrierRepository;
using TransitNova.Domain.Enums.Result;
namespace TransitNova.BusinessLayer.Features.Carriers.Commands.CommandsValidators
{
    public sealed class UpdateCarrierProfileCommandValidator : AbstractValidator<UpdateCarrierProfileCommand>
    {
        public UpdateCarrierProfileCommandValidator(IValidator<UpdateCarrierDto> dto,ICarrierRulesRepository carrier)
        {
            RuleFor(x => x.AppUserId)
                 .NotEmpty()
                .WithErrorCode($"{ErrorCode.UNAUTHORIZED}");

            RuleFor(x => x.Dto).SetValidator(dto);
        }
    }
}
