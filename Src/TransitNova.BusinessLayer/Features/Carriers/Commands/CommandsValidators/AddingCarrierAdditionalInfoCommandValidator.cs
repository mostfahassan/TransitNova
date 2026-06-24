using FluentValidation;
using TransitNova.BusinessLayer.DTOs.Carrier;
using TransitNova.BusinessLayer.Interfaces.Repositories.CarrierRepository;
using TransitNova.Domain.Enums.Result;

namespace TransitNova.BusinessLayer.Features.Carriers.Commands.CommandsValidators
{
    public sealed class AddingCarrierAdditionalInfoCommandValidator : AbstractValidator<AddingCarrierAdditionalInfoCommand>
    {
        public AddingCarrierAdditionalInfoCommandValidator(
            IValidator<AdditionalInfoDto> dtoValidator,
            ICarrierRulesRepository carrierRepository)
        {
            RuleFor(x => x.Dto)
                .NotNull()
                .SetValidator(dtoValidator);

            RuleFor(x => x.UserId)
                 .NotEmpty()
                 .WithErrorCode(ErrorCode.UNAUTHORIZED.ToString());

        }
    }
}
