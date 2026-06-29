
using FluentValidation;
using TransitNova.BusinessLayer.DTOs.UserProfile;
using TransitNova.BusinessLayer.Validators.BaseValidators;
namespace TransitNova.BusinessLayer.Validators.UserProfile.User
{
    public class UpdateUserValidator : BaseValidator<UpdateUserProfile>
    {
        public UpdateUserValidator()
        {
            When(x => x.UserBundleId != null, () =>
            {
                RuleFor(x => x.UserBundleId)
                    .NotEmpty();     
            });
        }
    }
    public class UpdateWarehouseManagerValidator : BaseValidator<UpdateWarehouseManagerProfile>
    {
        public UpdateWarehouseManagerValidator()
        {
            When(x => x.WarehouseId != null, () =>
            {
                RuleFor(x => x.WarehouseId)
                    .NotEmpty();     
            });
        }
    }
}
