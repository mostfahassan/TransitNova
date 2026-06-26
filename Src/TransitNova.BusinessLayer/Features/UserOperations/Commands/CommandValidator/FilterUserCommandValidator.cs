using FluentValidation;
using TransitNova.BusinessLayer.DTOs.UserProfile;
using TransitNova.BusinessLayer.Features.UserOperations.Queries;

namespace TransitNova.BusinessLayer.Features.UserOperations.Commands.CommandValidator
{
    public sealed class FilterUserCommandValidator : AbstractValidator<FilterUsersQuery>
    {
        public FilterUserCommandValidator(IValidator<UserFiltrationDto> dto)
        {
            RuleFor(x => x.FilterCriteria)
             .SetValidator(dto);
        }
        
    }
}
