using FluentValidation;
using TransitNova.BusinessLayer.DTOs.UserProfile;

namespace TransitNova.BusinessLayer.Features.UserOperations.Commands.CommandValidator
{
    public sealed class FilterUserCommandValidator : AbstractValidator<FilterUsersCommand>
    {
        public FilterUserCommandValidator(IValidator<UserFiltrationDto> dto)
        {
            RuleFor(x => x.FilterCriteria)
             .SetValidator(dto);
        }
        
    }
}
