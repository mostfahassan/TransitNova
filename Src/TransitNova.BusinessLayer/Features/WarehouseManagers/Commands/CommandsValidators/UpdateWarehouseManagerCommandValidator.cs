using FluentValidation;
using TransitNova.BusinessLayer.DTOs.UserProfile;
using TransitNova.BusinessLayer.Features.WarehouseManagers.Commands;
using TransitNova.Domain.Enums.Result;

namespace TransitNova.BusinessLayer.Features.WarehouseManagers.Commands.CommandsValidators
{
    public sealed class UpdateWarehouseManagerCommandValidator : AbstractValidator<UpdateWarehouseManagerCommand>
    {
        public UpdateWarehouseManagerCommandValidator(IValidator<UpdateWarehouseManagerProfile> dto)
        {
            RuleFor(x => x.Dto)
                .NotEmpty()
                .SetValidator(dto);
        }
    }
}

