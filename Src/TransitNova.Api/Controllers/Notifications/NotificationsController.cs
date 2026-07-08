using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TransitNova.BusinessLayer.Features.Notifications.Commands;
using TransitNova.BusinessLayer.Features.Notifications.Queries;

namespace TransitNova.Api.Controllers.Notifications
{
    [Authorize]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/notifications")]
    [Tags("Notifications")]
    public sealed class NotificationsController(IMediator mediator) : ControllerBase
    {
        [EnableRateLimiting("DefaultRateLimiter")]
        [HttpGet]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("Get Notifications")]
        [EndpointSummary("Get notifications for the authenticated user")]
        [EndpointDescription("Returns paginated notifications for the currently authenticated user, ordered from newest to oldest.")]
        public async Task<IActionResult> GetNotificationsAsync([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
        {
            var response = await mediator.Send(new GetNotificationsQuery(User.GetUserId(), pageNumber, pageSize), ct);
            return response.ToActionResult();
        }

        [EnableRateLimiting("DefaultRateLimiter")]
        [HttpGet("unread-count")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("Get Notifications Unread Count")]
        [EndpointSummary("Get unread notifications count for the authenticated user")]
        [EndpointDescription("Returns the unread notifications count scoped to the currently authenticated user.")]
        public async Task<IActionResult> GetUnreadCountAsync(CancellationToken ct = default)
        {
            var response = await mediator.Send(new GetUnreadNotificationsCountQuery(User.GetUserId()), ct);
            return response.ToActionResult();
        }

        [EnableRateLimiting("DefaultRateLimiter")]
        [HttpPatch("read-all")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("Mark All Notifications As Read")]
        [EndpointSummary("Mark all notifications as read for the authenticated user")]
        [EndpointDescription("Marks every unread notification belonging to the currently authenticated user as read.")]
        public async Task<IActionResult> MarkAllAsReadAsync(CancellationToken ct = default)
        {
            var response = await mediator.Send(new MarkAllNotificationsAsReadCommand(User.GetUserId()), ct);
            return response.ToActionResult();
        }
    }
}