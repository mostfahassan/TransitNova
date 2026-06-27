using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TransitNova.BusinessLayer.DTOs.Vehicle;
using TransitNova.BusinessLayer.Features.Vehicles.Commands;
using TransitNova.BusinessLayer.Features.Vehicles.Queries;
using TransitNova.Domain.Contracts.Permissions;
using TransitNova.Domain.Contracts.Roles;
using TransitNova.Api.Infrastructure.Idempotency;
namespace TransitNova.Api.Controllers.Admin.VehicleOperations
{
    [Authorize(Roles = Role.Admin)]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/admin/vehicles")]
    [Tags("Admin Vehicles")]
    public sealed class VehicleController(IMediator mediator) : ControllerBase
    {
        [Authorize(Policy = AdminPermissions.CreateVehicle)]
        [EnableRateLimiting("CommandsLimiter")]
        [HttpPost]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Consumes("application/json")]
        [EndpointName("Create Vehicle")]
        [EndpointSummary("Create a new vehicle")]
        [EndpointDescription("Creates a new vehicle and assigns it to a carrier.")]
        public async Task<IActionResult> CreateVehicleAsync([IdempotencyKey] Guid requestId, [FromBody] CreateVehicleDto dto, CancellationToken ct)
        {

            var response = await mediator.Send(new CreateVehicleCommand(requestId, dto), ct);
            return response.ToActionResult();
        }

        [Authorize(Policy = AdminPermissions.DeleteVehicle)]
        [EnableRateLimiting("CommandsLimiter")]
        [HttpDelete("{vehicleId:guid}")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("Delete Vehicle")]
        [EndpointSummary("Delete a vehicle")]
        [EndpointDescription("Deletes an existing vehicle from the system.")]
        public async Task<IActionResult> DeleteVehicleAsync([IdempotencyKey] Guid requestId, Guid vehicleId, CancellationToken ct)
        {

            var response = await mediator.Send(new DeleteVehicleCommand(requestId, vehicleId), ct);
            return response.ToActionResult();
        }

        [Authorize(Policy = AdminPermissions.ViewVehicles)]
        [EnableRateLimiting("DefaultRateLimiter")]
        [HttpGet]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("Get Vehicles")]
        [EndpointSummary("Get all vehicles")]
        [EndpointDescription("Returns all vehicles in the system.")]
        public async Task<IActionResult> VehiclesAsync(CancellationToken ct)
        {
            var response = await mediator.Send(new GetVehicleListQuery(), ct);
            return response.ToActionResult();
        }

        [Authorize(Policy = AdminPermissions.ViewVehicles)]
        [EnableRateLimiting("DefaultRateLimiter")]
        [HttpGet("active")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("Get Active Vehicles")]
        [EndpointSummary("Get active vehicles")]
        [EndpointDescription("Returns all active vehicles.")]
        public async Task<IActionResult> ActiveVehiclesAsync(CancellationToken ct)
        {
            var response = await mediator.Send(new GetActiveVehiclesQuery(), ct);
            return response.ToActionResult();
        }

        [Authorize(Policy = AdminPermissions.ViewVehicleDetails)]
        [EnableRateLimiting("DefaultRateLimiter")]
        [HttpGet("{vehicleId:guid}")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("Get Vehicle")]
        [EndpointSummary("Get vehicle details")]
        [EndpointDescription("Returns vehicle details by vehicle identifier.")]
        public async Task<IActionResult> VehicleAsync(Guid vehicleId, CancellationToken ct)
        {
            var response = await mediator.Send(new GetVehicleByIdQuery(vehicleId), ct);
            return response.ToActionResult();
        }

        [Authorize(Policy = AdminPermissions.ViewVehicles)]
        [EnableRateLimiting("DefaultRateLimiter")]
        [HttpGet("plate-number/{plateNumber}")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("Get Vehicle By Plate Number")]
        [EndpointSummary("Get vehicle using plate number")]
        [EndpointDescription("Returns vehicle details using the vehicle plate number.")]
        public async Task<IActionResult> VehicleByPlateNumberAsync(string plateNumber, CancellationToken ct)
        {
            var response = await mediator.Send(new GetVehicleByPlateNumberQuery(plateNumber), ct);
            return response.ToActionResult();
        }
    }
}
