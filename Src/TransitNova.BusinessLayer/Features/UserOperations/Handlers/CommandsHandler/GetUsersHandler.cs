using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.UserProfile;
using TransitNova.BusinessLayer.Features.UserOperations.Commands;
using TransitNova.BusinessLayer.Interfaces.Repositories.UserRepository;
namespace TransitNova.BusinessLayer.Features.UserOperations.Handlers.CommandsHandler
{
    public sealed class GetUsersHandler(
        IUserQueryRepository userQueryRepository,
        ILogger<GetUsersHandler> logger)
        : IQueryHandler<FilterUsersCommand, Result<PagedResult<AdminUserDetailsDto>>>
    {
        public async Task<Result<PagedResult<AdminUserDetailsDto>>> Handle(FilterUsersCommand request, CancellationToken ct)
        {
            logger.LogInformation("Filtering users with criteria: {@FilterCriteria}", request.FilterCriteria);

            var users = await userQueryRepository.FilterUsersAsync(request.FilterCriteria, ct);
            if (!users.Data.Any())
            {
                logger.LogInformation("No users found matching the filter criteria");
                var emptyResult = Result<PagedResult<AdminUserDetailsDto>>.Success(users);
                return emptyResult;
            }

            logger.LogInformation("Found {Count} users (Page {PageNumber}/{TotalPages})",
                users.Data.Count(),
                users.PageNumber,
                users.TotalPages);

            var result = Result<PagedResult<AdminUserDetailsDto>>.Success(users);
            return result;
        }
    }
}
