using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TransitNova.BusinessLayer.DTOs.Warehouse;
using TransitNova.BusinessLayer.Features.Warehouses.Commands;
using TransitNova.BusinessLayer.Features.Warehouses.Queries;
using TransitNova.Domain.Contracts.Permissions;
using TransitNova.Domain.Contracts.Roles;
namespace TransitNova.Api.Controllers.Admin.WarehouseOperations
{
    [Authorize(Roles = Role.Admin)]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/admin/warehouses")]
    [Tags("Admin Warehouses")]
    public sealed class WarehouseController(IMediator mediator) : ControllerBase
    {
        [Authorize(Policy = AdminPermissions.CreateWarehouse)]
        [EnableRateLimiting("CommandsLimiter")]
        [HttpPost]
        [MapToApiVersion("1.0")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("Create Warehouse")]
        [EndpointSummary("Create a new warehouse")]
        [EndpointDescription("Creates a new warehouse and optionally links it to served zones.")]
        public async Task<IActionResult> CreateWarehouseAsync([FromHeader(Name = "X-Idempotency-Key")] string requestId, [FromBody] CreateWarehouseDto dto, CancellationToken ct)
        {
            if (!Guid.TryParse(requestId, out Guid parsedRequestId))
                return BadRequest();

            var adminId = User.GetUserId();
            var response = await mediator.Send(new CreateWarehouseCommand(parsedRequestId, adminId, dto), ct);
            return response.ToActionResult();
        }

        [Authorize(Policy = AdminPermissions.UpdateWarehouse)]
        [EnableRateLimiting("CommandsLimiter")]
        [HttpPut("{warehouseId:guid}")]
        [MapToApiVersion("1.0")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("Update Warehouse")]
        [EndpointSummary("Update an existing warehouse")]
        [EndpointDescription("Updates warehouse details and served zone assignments.")]
        public async Task<IActionResult> UpdateWarehouseAsync([FromHeader(Name = "X-Idempotency-Key")] string requestId, Guid warehouseId, [FromBody] UpdateWarehouseDto dto, CancellationToken ct)
        {
            if (!Guid.TryParse(requestId, out Guid parsedRequestId))
                return BadRequest();

            var adminId = User.GetUserId();
            var response = await mediator.Send(new UpdateWarehouseCommand(parsedRequestId, warehouseId, adminId, dto), ct);
            return response.ToActionResult();
        }

        [Authorize(Policy = AdminPermissions.DeleteWarehouse)]
        [EnableRateLimiting("CommandsLimiter")]
        [HttpDelete("{warehouseId:guid}")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("Delete Warehouse")]
        [EndpointSummary("Delete a warehouse")]
        [EndpointDescription("Deletes an existing warehouse from the system.")]
        public async Task<IActionResult> DeleteWarehouseAsync([FromHeader(Name = "X-Idempotency-Key")] string requestId, Guid warehouseId, CancellationToken ct)
        {
            if (!Guid.TryParse(requestId, out Guid parsedRequestId))
                return BadRequest();

            var adminId = User.GetUserId();
            var response = await mediator.Send(new DeleteWarehouseCommand(parsedRequestId, warehouseId, adminId), ct);
            return response.ToActionResult();
        }

        [Authorize(Policy = AdminPermissions.ViewWarehouses)]
        [EnableRateLimiting("DefaultRateLimiter")]
        [HttpGet]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("Get Warehouses")]
        [EndpointSummary("Get all warehouses")]
        [EndpointDescription("Returns all warehouses in the system.")]
        public async Task<IActionResult> WarehousesAsync(CancellationToken ct)
        {
            var response = await mediator.Send(new GetWarehouseListQuery(), ct);
            return response.ToActionResult();
        }

        [Authorize(Policy = AdminPermissions.ViewWarehouseDetails)]
        [EnableRateLimiting("DefaultRateLimiter")]
        [HttpGet("{warehouseId:guid}")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("Get Warehouse")]
        [EndpointSummary("Get warehouse details")]
        [EndpointDescription("Returns warehouse details by warehouse identifier.")]
        public async Task<IActionResult> WarehouseAsync(Guid warehouseId, CancellationToken ct)
        {
            var response = await mediator.Send(new GetWarehouseByIdQuery(warehouseId), ct);
            return response.ToActionResult();
        }
    }
}
