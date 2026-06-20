using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TransitNova.BusinessLayer.Features.UserOperations.Queries;
using TransitNova.Domain.Contracts.Permissions;
using TransitNova.Domain.Contracts.Roles;
namespace TransitNova.Api.Controllers.User.Profile
{
    [Authorize(Roles = Role.User)]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/user")]
    [Tags("User Profile Controller")]
    public sealed class ProfileController(IMediator mediator) : ControllerBase
    {
        [EnableRateLimiting("DefaultRateLimiter")]
        [HttpGet("dashboard")]
        [Authorize(Policy = UserPermissions.UserCanShowHisDashboard)]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("Get User Dashboard")]
        [EndpointSummary("Get dashboard information for the authenticated user")]
        [EndpointDescription("Returns the dashboard information associated with the authenticated user,including shipment statistics, activity summaries, and other user-specific operational data.")]
        public async Task<IActionResult> Dashboard(CancellationToken ct)
        {
            var userId = User.GetUserId();
            var response = await mediator.Send(new GetUserDashboardQuery(userId), ct);
            return response.ToActionResult();
        }
        
        [EnableRateLimiting("DefaultRateLimiter")]
        [HttpGet("profile")]
        [Authorize(Policy = UserPermissions.UserCanShowHisDashboard)]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("Get User Profile")]
        [EndpointSummary("Get the authenticated user's profile")]
        [EndpointDescription("Returns the profile details of the authenticated user, including account information, contact details, profile settings, and other user-specific data.")]
        public async Task<IActionResult> Profile(CancellationToken ct)
        {
            var userId = User.GetUserId();
            var response = await mediator.Send(new GetUserProfileQuery(userId), ct);
            return response.ToActionResult();
        }


    }

}

