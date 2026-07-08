using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TransitNova.BusinessLayer.Features.Warehouses.Queries;
using TransitNova.Domain.Contracts.Roles;

namespace TransitNova.Api.Controllers.Warehouses;

[Authorize(Roles = Role.AllUsers)]
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/warehouses")]
[Tags("Warehouses")]
public sealed class WarehousesController(IMediator mediator) : ControllerBase
{
    [EnableRateLimiting("DefaultRateLimiter")]
    [HttpGet]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [EndpointName("Get Shared Warehouses")]
    [EndpointSummary("Get warehouses available to authenticated users")]
    [EndpointDescription("Returns the warehouse list for authenticated UI flows that need read-only warehouse selection without admin permissions.")]
    public async Task<IActionResult> WarehousesAsync(CancellationToken ct)
    {
        var response = await mediator.Send(new GetWarehouseListQuery(), ct);
        return response.ToActionResult();
    }
}