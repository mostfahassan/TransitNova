using FluentValidation;
using TransitNova.BusinessLayer.DTOs.Carrier;
using TransitNova.Domain.Enums.Result;
namespace TransitNova.BusinessLayer.Validators.CarrierRatingValidators
{
    public class RatingCarrierDtoValidator : AbstractValidator<RatingCarrierDto>
    {
        public RatingCarrierDtoValidator()
        {
            RuleFor(x => x.CarrierId)
                .NotEmpty()
                .WithErrorCode($"{ErrorCode.REQUIRED_FIELD}");

            RuleFor(x => x.Rating)
                .InclusiveBetween(1, 5)
                .WithMessage("Rating must be between 1 and 5.");

            RuleFor(x => x.Comment)
                .MaximumLength(500)
                .WithMessage("Comment cannot exceed 250 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.Comment));
        }
    }
}