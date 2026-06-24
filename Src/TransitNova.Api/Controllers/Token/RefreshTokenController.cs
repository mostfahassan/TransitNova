
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TransitNova.BusinessLayer.DTOs.RefreshToken;
using TransitNova.BusinessLayer.Features.Token.Commands;

namespace TransitNova.Api.Controllers.Token
{
    [AllowAnonymous]
    [Route("api/v{version:apiVersion}/refresh-tokens")]
    [ApiVersion("1.0")]
    [ApiController]
    [Tags("Tokens")]
    public class RefreshTokenController(IMediator mediator,IAuthorizationService authorizationService) : ControllerBase
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
        public async Task<IActionResult> GenerateRefreshTokenAsync([FromHeader(Name = "X-Idempotency-Key")] string requestId, [FromBody] RefreshToken refreshToken, CancellationToken ct)
        {
            if (!Guid.TryParse(requestId, out Guid parsedRequestId))
                return BadRequest();
            if (!await UserOwnsRefreshTokenAsync(refreshToken.Token))
                return Forbid();

            var response = await mediator.Send(new GenerateRefreshTokenCommand(parsedRequestId, refreshToken.Token), ct);
            return response.ToActionResult();
        }

        [EnableRateLimiting("CommandsLimiter")]
        [HttpDelete("{userId:guid}")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("Revoke Refresh Token")]
        [EndpointSummary("Revoke user authentication tokens")]
        [EndpointDescription("Revokes the user's active refresh token and invalidates the current authentication session. Once revoked, the refresh token can no longer be used to generate new access tokens.Users must authenticate again to obtain new authentication tokens")]
        public async Task<IActionResult> RevokeRefreshTokenAsync([FromHeader(Name = "X-Idempotency-Key")] string requestId, Guid userId, CancellationToken ct)
        {
            if (!Guid.TryParse(requestId, out Guid parsedRequestId))
                return BadRequest();
            if (!await IsTokenOwner(userId))
                return Forbid();

            var response = await mediator.Send(new RevokeRefreshTokenCommand(parsedRequestId, userId), ct);
            return response.ToActionResult();
        }
        private async Task<bool> UserOwnsRefreshTokenAsync(string token)
        {
            var authorizationResult =
               await authorizationService.AuthorizeAsync(
                   User,
                   token,
                  "RefreshTokenOwner");

            return authorizationResult.Succeeded;
        }
        private async Task<bool> IsTokenOwner(Guid Id)
        {
            var authorizationResult =
               await authorizationService.AuthorizeAsync(
                   User, 
                   Id,
                  "IsTokenOwner");

            return authorizationResult.Succeeded;
        }


    }
}
