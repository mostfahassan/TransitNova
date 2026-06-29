using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TransitNova.BusinessLayer.DTOs.WarehouseManager;
using TransitNova.BusinessLayer.Features.WarehouseManagers.Queries;
using TransitNova.Domain.Contracts.Roles;

namespace TransitNova.Api.Controllers.Admin.WarehouseManager
{
    [Authorize(Roles = Role.Admin)]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/admin/warehouse-managers")]
    [Tags("Admin Warehouse Managers")]
    public sealed class WarehouseManagerAdminController(IMediator mediator) : ControllerBase
    {
        [EnableRateLimiting("DefaultRateLimiter")]
        [HttpGet]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("Get Warehouse Managers")]
        [EndpointSummary("Get all warehouse managers")]
        [EndpointDescription("Returns a paginated list of warehouse managers.")]
        public async Task<IActionResult> WarehouseManagersAsync([FromQuery] WarehouseManagerFilterDto filter, CancellationToken ct)
        {
            var response = await mediator.Send(new GetAllWarehouseManagersQuery(filter), ct);
            return response.ToActionResult();
        }

        [EnableRateLimiting("DefaultRateLimiter")]
        [HttpGet("{id:guid}")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("Get Warehouse Manager")]
        [EndpointSummary("Get warehouse manager details")]
        [EndpointDescription("Returns warehouse manager details by identifier.")]
        public async Task<IActionResult> WarehouseManagerAsync(Guid id, CancellationToken ct)
        {
            var response = await mediator.Send(new GetWarehouseManagerByIdQuery(id), ct);
            return response.ToActionResult();
        }

       
    }
}
