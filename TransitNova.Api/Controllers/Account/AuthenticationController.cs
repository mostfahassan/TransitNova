using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TransitNova.BusinessLayer.DTOs.UserProfile.Auth;
using TransitNova.BusinessLayer.Features.UserAuthentication.Authentication.Commands;
using TransitNova.Domain.Contracts.Roles;
namespace TransitNova.Api.Controllers.Account
{
    [Route("api/auth")]
    [ApiController]
    [Tags("Authentication")]
    public sealed class AuthenticationController(IMediator mediator) : ControllerBase
    {
        [EnableRateLimiting("CommandsLimiter")]
        [HttpPost("register")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("Register User")]
        [EndpointSummary("Create a new user account")]
        [EndpointDescription("Creates a new user account using the supplied registration information. A successful registration returns the newly created user details.")] 
        public async Task<IActionResult> Register([FromHeader(Name = "X-Idempotency-Key")] string requestId, RegisterDto register, CancellationToken ct)
        {
            if (!Guid.TryParse(requestId, out Guid parsedRequestId))
                return BadRequest();

            var response = await mediator.Send(new RegistrationCommand(parsedRequestId, register), ct);
            return response.ToActionResult();
        }


        [EnableRateLimiting("CommandsLimiter")]
        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("Login User")]
        [EndpointSummary("Authenticate a user")]
        [EndpointDescription("Authenticates a user using valid credentials and returns the authentication tokens and account information.")]
        public async Task<IActionResult> Login([FromHeader(Name = "X-Idempotency-Key")] string requestId, LoginDto login, CancellationToken ct)
        {
            if (!Guid.TryParse(requestId, out Guid parsedRequestId))
                return BadRequest();

            var response = await mediator.Send(new LoginCommand(parsedRequestId, login), ct);
            return response.ToActionResult();
        }

        [Authorize(Roles = Role.AllUsers)]
        [EnableRateLimiting("CommandsLimiter")]
        [HttpPut("change-password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("Change Password")]
        [EndpointSummary("Change the current user password")]
        [EndpointDescription("Allows an authenticated user to change their account password by providing the current password and a new password.")]
        public async Task<IActionResult> ChangePassword([FromHeader(Name = "X-Idempotency-Key")] string requestId, ChangePasswordDto changePassword, CancellationToken ct)
        {
            if (!Guid.TryParse(requestId, out Guid parsedRequestId))
                return BadRequest();

            var userId = User.GetUserId();
            var response = await mediator.Send(new ChangePasswordCommand(parsedRequestId, changePassword, userId), ct);
            return response.ToActionResult();
        }

        [Authorize(Roles = Role.AllUsers)]
        [EnableRateLimiting("CommandsLimiter")]
        [HttpPost("signout")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("Sign Out User")]
        [EndpointSummary("Sign out the current user")]
        [EndpointDescription("Terminates the current authenticated session and invalidates the user's active authentication context.")]
        public async Task<IActionResult> SignOut([FromHeader(Name = "X-Idempotency-Key")] string requestId, CancellationToken ct)
        {
            if (!Guid.TryParse(requestId, out Guid parsedRequestId))
                return BadRequest();

            var response = await mediator.Send(new SignOutCommand(parsedRequestId), ct);
            return response.ToActionResult();
        }
    }
}
