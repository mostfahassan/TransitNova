using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TransitNova.BusinessLayer.Features.OperationManagerService.Queries.OperationManagerQueries;
using TransitNova.Domain.Contracts.Permissions;
using TransitNova.Domain.Contracts.Roles;

namespace TransitNova.Api.Controllers.Admin.OperationManager
{
    [Authorize(Roles = Role.Admin)]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/admin/operation-managers")]
    [Tags("Admin Operation Managers")]
    public sealed class OperationManagersController(IMediator mediator) : ControllerBase
    {
        [Authorize(Policy = AdminPermissions.ViewOperationManagers)]
        [EnableRateLimiting("DefaultRateLimiter")]
        [HttpGet]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("Get Operation Managers")]
        [EndpointSummary("Get all operation managers")]
        [EndpointDescription("Returns all operation managers in the system.")]
        public async Task<IActionResult> OperationManagers(CancellationToken ct)
        {
            var response = await mediator.Send(new GetAllManagersQuery(), ct);
            return response.ToActionResult();
        }

        [Authorize(Policy = AdminPermissions.ViewOperationManagers)]
        [EnableRateLimiting("DefaultRateLimiter")]
        [HttpGet("active")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("Get Active Operation Managers")]
        [EndpointSummary("Get active operation managers")]
        [EndpointDescription("Returns all active operation managers in the system.")]
        public async Task<IActionResult> ActiveOperationManagers(CancellationToken ct)
        {
            var response = await mediator.Send(new GetActiveManagerQuery(), ct);
            return response.ToActionResult();
        }

        [Authorize(Policy = AdminPermissions.ViewOperationManagerDetails)]
        [EnableRateLimiting("DefaultRateLimiter")]
        [HttpGet("{operationManagerId:guid}")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("Get Operation Manager")]
        [EndpointSummary("Get operation manager details")]
        [EndpointDescription("Returns operation manager details by operation manager identifier.")]
        public async Task<IActionResult> OperationManager(Guid operationManagerId, CancellationToken ct)
        {
            var response = await mediator.Send(new GetOperationManagerDetailsQuery(operationManagerId), ct);
            return response.ToActionResult();
        }

        [Authorize(Policy = AdminPermissions.ViewOperationManagerDetails)]
        [EnableRateLimiting("DefaultRateLimiter")]
        [HttpGet("{operationManagerId:guid}/handled-carriers")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("Get Operation Manager Handled Carriers")]
        [EndpointSummary("Get operation manager handled carriers")]
        [EndpointDescription("Returns carriers handled by the specified operation manager.")]
        public async Task<IActionResult> HandledCarriers(
            Guid operationManagerId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20,
            CancellationToken ct = default)
        {
            var response = await mediator.Send(new GetOperationManagerHandledCarriersQuery(operationManagerId, pageNumber, pageSize), ct);
            return response.ToActionResult();
        }

        [Authorize(Policy = AdminPermissions.ViewOperationManagerDetails)]
        [EnableRateLimiting("DefaultRateLimiter")]
        [HttpGet("{operationManagerId:guid}/handled-shipments")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("Get Operation Manager Handled Shipments")]
        [EndpointSummary("Get operation manager handled shipments")]
        [EndpointDescription("Returns shipments handled by the specified operation manager.")]
        public async Task<IActionResult> HandledShipments(
            Guid operationManagerId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20,
            CancellationToken ct = default)
        {
            var response = await mediator.Send(new GetOperationManagerHandledShipmentsQuery(operationManagerId, pageNumber, pageSize), ct);
            return response.ToActionResult();
        }
    }
}
