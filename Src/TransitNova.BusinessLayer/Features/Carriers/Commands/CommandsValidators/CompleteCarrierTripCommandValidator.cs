using FluentValidation;
using TransitNova.Domain.Enums.Result;

namespace TransitNova.BusinessLayer.Features.Carriers.Commands.CommandsValidators
{
    public sealed class CompleteCarrierTripCommandValidator : AbstractValidator<CompleteCarrierTripCommand>
    {
        public CompleteCarrierTripCommandValidator()
        {
            RuleFor(x => x.CarrierId)
                .NotEmpty()
                .WithErrorCode($"{ErrorCode.UNAUTHORIZED}");

            RuleFor(x => x.TripId)
                .NotEmpty()
                .WithErrorCode($"{ErrorCode.REQUIRED_FIELD}");
        }
    }
}