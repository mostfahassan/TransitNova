using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TransitNova.BusinessLayer.Features.OperationManagerService.Queries.OperationManagerQueries;
using TransitNova.Domain.Contracts.Permissions;
using TransitNova.Domain.Contracts.Roles;

namespace TransitNova.Api.Controllers.OperationManager.Query.OperationManager
{
    [Authorize(Roles = Role.OperationManagerOrAdmin)]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/operation-managers")]
    [Tags("Operation Managers")]
    public sealed class OperationManagersController(IMediator mediator) : ControllerBase
    {
        [Authorize(Policy = OperationManagerPermissions.ViewProfile)]
        [EnableRateLimiting("DefaultRateLimiter")]
        [HttpGet("{operationManagerId:guid}")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("Get Operation Manager Details")]
        [EndpointSummary("Get operation manager details")]
        [EndpointDescription("Returns operation manager details by operation manager identifier.")]
        public async Task<IActionResult> OperationManagerAsync(Guid operationManagerId, CancellationToken ct)
        {
            var response = await mediator.Send(new GetOperationManagerDetailsQuery(operationManagerId), ct);
            return response.ToActionResult();
        }

        [Authorize(Policy = OperationManagerPermissions.ViewCarriers)]
        [EnableRateLimiting("DefaultRateLimiter")]
        [HttpGet("{operationManagerId:guid}/handled-carriers")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("Get Handled Carriers")]
        [EndpointSummary("Get handled carriers")]
        [EndpointDescription("Returns carriers handled by the specified operation manager.")]
        public async Task<IActionResult> HandledCarriersAsync(Guid operationManagerId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
        {
            var response = await mediator.Send(new GetOperationManagerHandledCarriersQuery(operationManagerId, pageNumber, pageSize), ct);
            return response.ToActionResult();
        }

        [Authorize(Policy = OperationManagerPermissions.ViewAllShipments)]
        [EnableRateLimiting("DefaultRateLimiter")]
        [HttpGet("{operationManagerId:guid}/handled-shipments")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("Get Handled Shipments")]
        [EndpointSummary("Get handled shipments")]
        [EndpointDescription("Returns shipments handled by the specified operation manager.")]
        public async Task<IActionResult> HandledShipmentsAsync(Guid operationManagerId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20,CancellationToken ct = default)
        {
            var response = await mediator.Send(new GetOperationManagerHandledShipmentsQuery(operationManagerId, pageNumber, pageSize), ct);
            return response.ToActionResult();
        }
    }
}
