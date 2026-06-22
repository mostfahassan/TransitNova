
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TransitNova.BusinessLayer.DTOs.RefreshToken;
using TransitNova.BusinessLayer.Features.Token.Commands;
using TransitNova.Domain.Contracts.Roles;
namespace TransitNova.Api.Controllers.Token
{
    [Authorize(Roles = Role.AllUsers)]
    [Route("api/v{version:apiVersion}/refresh-tokens")]
    [ApiVersion("1.0")]
    [ApiController]
    [Tags("Tokens")]
    public class RefreshTokenController(IMediator mediator) : ControllerBase
    {
        [EnableRateLimiting("CommandsLimiter")]
        [HttpPost]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Consumes("application/json")]
        [EndpointName("Refresh Token")]
        [EndpointSummary("Refresh user authentication tokens")]
        [EndpointDescription("Allows a user to obtain a new access token without re-authenticating. The endpoint validates the supplied refresh token, generates a new JWT access token, " +
             "issues a new refresh token, and returns the updated authentication information. An invalid or expired refresh token will result in an unauthorized response.")]
        public async Task<IActionResult> GenerateRefreshToken([FromHeader(Name = "X-Idempotency-Key")] string requestId, RefreshToken refreshToken, CancellationToken ct)
        {
            if (!Guid.TryParse(requestId, out Guid parsedRequestId))
                return BadRequest();

            var response = await mediator.Send(new GenerateRefreshTokenCommand(parsedRequestId, refreshToken.Token), ct);
            return response.ToActionResult();
        }

        [EnableRateLimiting("CommandsLimiter")]
        [HttpDelete("{id:guid}")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("Revoke Refresh Token")]
        [EndpointSummary("Revoke user authentication tokens")]
        [EndpointDescription("Revokes the user's active refresh token and invalidates the current authentication session. Once revoked, the refresh token can no longer be used to generate new access tokens.Users must authenticate again to obtain new authentication tokens")]
        public async Task<IActionResult> RevokeRefreshToken([FromHeader(Name = "X-Idempotency-Key")] string requestId, Guid id, CancellationToken ct)
        {
            if (!Guid.TryParse(requestId, out Guid parsedRequestId))
                return BadRequest();

            var response = await mediator.Send(new RevokeRefreshTokenCommand(parsedRequestId, id), ct);
            return response.ToActionResult();
        }

 
    }
}
