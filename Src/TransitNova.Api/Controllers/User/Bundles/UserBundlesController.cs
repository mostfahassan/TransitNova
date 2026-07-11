using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TransitNova.BusinessLayer.Features.Bundles.Queries;
using TransitNova.Domain.Contracts.Permissions;
using TransitNova.Domain.Contracts.Roles;

namespace TransitNova.Api.Controllers.User.Bundles;

[Authorize(Roles = Role.User)]
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/users/bundles")]
[Tags("User Bundle Catalog")]
public sealed class UserBundlesController(IMediator mediator) : ControllerBase
{
    [Authorize(Policy = UserPermissions.UserCanViewBundles)]
    [EnableRateLimiting("DefaultRateLimiter")]
    [HttpGet]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [EndpointName("Get User Bundles")]
    [EndpointSummary("Get bundle catalog for authenticated users")]
    [EndpointDescription("Returns the bundle catalog available for authenticated users to review before checkout.")]
    public async Task<IActionResult> BundlesAsync(CancellationToken ct)
    {
        var response = await mediator.Send(new GetBundleListQuery(), ct);
        return response.ToActionResult();
    }

    [Authorize(Policy = UserPermissions.UserCanViewBundles)]
    [EnableRateLimiting("DefaultRateLimiter")]
    [HttpGet("{bundleId:guid}")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [EndpointName("Get User Bundle")]
    [EndpointSummary("Get bundle details for authenticated users")]
    [EndpointDescription("Returns a single bundle by id for the authenticated user's checkout screen.")]
    public async Task<IActionResult> BundleAsync(Guid bundleId, CancellationToken ct)
    {
        var response = await mediator.Send(new GetBundleByIdQuery(bundleId), ct);
        return response.ToActionResult();
    }
}
