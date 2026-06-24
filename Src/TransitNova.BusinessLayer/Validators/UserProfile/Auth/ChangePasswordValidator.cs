using FluentValidation;
using TransitNova.BusinessLayer.DTOs.UserProfile.Auth;
namespace TransitNova.BusinessLayer.Validators.UserProfile.Auth
{
    public class ChangePasswordValidator : AbstractValidator<ChangePasswordDto>
    {
        public ChangePasswordValidator()
        {
            RuleFor(x => x.CurrentPassword)
                .NotEmpty().WithMessage("Current password is required.")
                .MinimumLength(6).WithMessage("Current password must be at least 6 characters long.");

            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("New password is required.")
                .MinimumLength(6).WithMessage("New password must be at least 6 characters long.");

            RuleFor(x => x.ConfirmNewPassword)
                .Equal(x => x.NewPassword).WithMessage("Passwords do not match.");
        }
    }
}