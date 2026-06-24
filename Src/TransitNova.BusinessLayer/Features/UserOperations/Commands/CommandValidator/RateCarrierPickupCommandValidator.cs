
using FluentValidation;
using TransitNova.BusinessLayer.DTOs.Carrier;
using TransitNova.Domain.Enums.Result;
namespace TransitNova.BusinessLayer.Features.UserOperations.Commands.CommandValidator
{
    public sealed class RateCarrierPickupCommandValidator:AbstractValidator<RatePickupCarrierCommand>
    {
        public RateCarrierPickupCommandValidator(IValidator<RatingCarrierDto> dto)
        {
            RuleFor(x => x.AppUserId)
                .NotEmpty()
                .WithErrorCode($"{ErrorCode.REQUIRED_FIELD}");
            
            RuleFor(x => x.shipmentId)
                .NotEmpty()
                .WithErrorCode($"{ErrorCode.REQUIRED_FIELD}");

            RuleFor(x => x.Dto)
                .NotEmpty()
                .SetValidator(dto);
        }

    }
}
