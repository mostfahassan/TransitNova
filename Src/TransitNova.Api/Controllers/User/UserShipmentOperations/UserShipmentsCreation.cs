
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TransitNova.BusinessLayer.DTOs.Shipment;
using TransitNova.BusinessLayer.Features.UserOperations.Commands.Shipment;
using TransitNova.Domain.Contracts.Permissions;
using TransitNova.Domain.Contracts.Roles;
using TransitNova.Api.Infrastructure.Idempotency;
namespace TransitNova.Api.Controllers.User.UserShipmentOperations
{
    [Authorize(Roles = Role.User)]
    [Route("api/v{version:apiVersion}/users/shipments")]
    [ApiVersion("1.0")]
    [ApiController]
    [Tags("User Shipments")]
    public class UserShipmentsCreation(IMediator mediator) : ControllerBase
    {
        
        // POST api/v1/shipments
        [Authorize(Policy = UserPermissions.UserCanAddShipment)]
        [EnableRateLimiting("CommandsLimiter")]
        [HttpPost()] 
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Consumes("application/json")]
        [EndpointName("CreateShipment")]
        [EndpointSummary("Creates a new shipment for the authenticated user.")]
        [EndpointDescription("This endpoint allows an authenticated user to create a new shipment by providing the necessary shipment details in the request body. The user must have the 'User.AddShipment' permission to access this endpoint.")]
        public async Task<IActionResult> CreateShipmentAsync([IdempotencyKey] Guid requestId ,[FromBody] CreateShipmentDto dto, CancellationToken ct)
        {
            var userId = User.GetUserId();
            var response = await mediator.Send(new CreateShipmentCommand(requestId,dto, userId), ct);
            return response.ToActionResult();
        }
    }
}
