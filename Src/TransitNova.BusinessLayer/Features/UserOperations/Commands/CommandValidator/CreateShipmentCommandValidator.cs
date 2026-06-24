using FluentValidation;
using TransitNova.BusinessLayer.DTOs.Shipment;
using TransitNova.Domain.Enums.Result;
namespace TransitNova.BusinessLayer.Features.UserOperations.Commands.CommandValidator
{
    public sealed class CreateShipmentCommandValidator : AbstractValidator<CreateShipmentCommand>
    {
        public CreateShipmentCommandValidator(IValidator<CreateShipmentDto> dtoValidator)
        {
            RuleFor(x => x.Dto)
                .NotNull()
                .SetValidator(dtoValidator);

            RuleFor(x => x.AppUserId)
                .NotEmpty()
                .WithErrorCode($"{ErrorCode.UNAUTHORIZED}");

        }
    }
}
