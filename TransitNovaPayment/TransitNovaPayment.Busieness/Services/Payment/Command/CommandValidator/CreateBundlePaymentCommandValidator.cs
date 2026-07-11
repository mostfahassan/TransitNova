using FluentValidation;
using TransitNovaPayment.Busieness.Common.ResultResponse.Result.ErrorsResult.Enum;
using TransitNovaPayment.Busieness.DTO.PaymentDto;

namespace TransitNovaPayment.Busieness.Services.Payment.Command.CommandValidator
{
    public sealed class CreateBundlePaymentCommandValidator : AbstractValidator<CreateBundlePaymentCommand>
    {
        public CreateBundlePaymentCommandValidator(IValidator<CreatePaymentDto> dto)
        {
            RuleFor(x => x.Dto)
                .NotEmpty()
                .SetValidator(dto);

            RuleFor(x => x.Key)
                .NotEmpty()
                .WithErrorCode($"{ErrorCode.UNAUTHORIZED}");
        }
    }
}
