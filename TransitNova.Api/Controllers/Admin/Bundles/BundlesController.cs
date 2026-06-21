using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TransitNova.BusinessLayer.DTOs.Bundle;
using TransitNova.BusinessLayer.Features.Bundles.Commands;
using TransitNova.BusinessLayer.Features.Bundles.Queries;
using TransitNova.Domain.Contracts.Permissions;
using TransitNova.Domain.Contracts.Roles;

namespace TransitNova.Api.Controllers.Admin.Bundles
{
    [Authorize(Roles = Role.Admin)]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/admin/bundles")]
    [Tags("Bundle Management")]
    public sealed class BundleController(IMediator mediator) : ControllerBase
    {
        [Authorize(Policy = AdminPermissions.CreateBundle)]
        [EnableRateLimiting("CommandsLimiter")]
        [HttpPost]
        [MapToApiVersion("1.0")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("Create Bundle")]
        [EndpointSummary("Create a new bundle")]
        [EndpointDescription("Creates a new bundle and makes it available for user subscriptions.")]
        public async Task<IActionResult> Create([FromHeader(Name = "X-Idempotency-Key")] string requestId, [FromBody] CreateBundleDto dto, CancellationToken ct)
        {
            if (!Guid.TryParse(requestId, out Guid parsedRequestId))
                return BadRequest();

            var adminId = User.GetUserId();
            var response = await mediator.Send(new CreateBundleCommand(parsedRequestId, adminId, dto), ct);
            return response.ToActionResult();
        }

        [Authorize(Policy = AdminPermissions.UpdateBundle)]
        [EnableRateLimiting("CommandsLimiter")]
        [HttpPut]
        [MapToApiVersion("1.0")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("Update Bundle")]
        [EndpointSummary("Update an existing bundle")]
        [EndpointDescription("Updates the details of an existing bundle.")]
        public async Task<IActionResult> Update([FromHeader(Name = "X-Idempotency-Key")] string requestId, [FromBody] UpdateBundleDto dto, CancellationToken ct)
        {
            if (!Guid.TryParse(requestId, out Guid parsedRequestId))
                return BadRequest();

            var adminId = User.GetUserId();
            var response = await mediator.Send(new UpdateBundleCommand(parsedRequestId, dto, adminId), ct);
            return response.ToActionResult();
        }

        [Authorize(Policy = AdminPermissions.DeleteBundle)]
        [EnableRateLimiting("CommandsLimiter")]
        [HttpDelete("{bundleId:guid}")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("Delete Bundle")]
        [EndpointSummary("Delete a bundle")]
        [EndpointDescription("Deletes an existing bundle from the system.")]
        public async Task<IActionResult> Delete([FromHeader(Name = "X-Idempotency-Key")] string requestId, Guid bundleId, CancellationToken ct)
        {
            if (!Guid.TryParse(requestId, out Guid parsedRequestId))
                return BadRequest();

            var response = await mediator.Send(new DeleteBundleCommand(parsedRequestId, bundleId), ct);
            return response.ToActionResult();
        }

        [Authorize(Policy = AdminPermissions.ViewBundles)]
        [EnableRateLimiting("DefaultRateLimiter")]
        [HttpGet]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("Get Bundles")]
        [EndpointSummary("Get all bundles")]
        [EndpointDescription("Returns all bundles available in the system.")]
        public async Task<IActionResult> Bundles(CancellationToken ct)
        {
            var response = await mediator.Send(new GetBundleListQuery(), ct);
            return response.ToActionResult();
        }

        [Authorize(Policy = AdminPermissions.ViewBundleDetails)]
        [EnableRateLimiting("DefaultRateLimiter")]
        [HttpGet("{bundleId:guid}")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("Get Bundle")]
        [EndpointSummary("Get bundle details")]
        [EndpointDescription("Returns the details of a specific bundle.")]
        public async Task<IActionResult> Bundle(Guid bundleId, CancellationToken ct)
        {
            var response = await mediator.Send(new GetBundleByIdQuery(bundleId), ct);
            return response.ToActionResult();
        }
    }
}
