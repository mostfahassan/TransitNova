using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.DTOs.AppUser;
using TransitNova.BusinessLayer.Interfaces.Services.IdentityOperationService;

namespace TransitNova.InfraStructure.Common.IdentityService
{
    internal class UserAuthRulesService(IMapper mapper, 
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        ILogger<UserAuthRulesService> logger) : IUserAuthRulesService
    {
        public async Task<bool> IsUserLockedAsync(AppUserDto userDto, CancellationToken ct)
        {
            logger.LogTrace("Checking lock status for User ID: {UserId}", userDto.Id);
            var appUser = await userManager.FindByIdAsync(userDto.Id.ToString());
            return appUser is not null && await userManager.IsLockedOutAsync(appUser);
        }

        public async Task<(bool, AppUserDto?, string)> ValidatePasswordAsync(string email, string password, bool lockoutOnFailure, CancellationToken ct)
        {
            logger.LogTrace("Validating password for User ID: {UserId}", email);
            var appUser = await userManager.FindByEmailAsync(email);
            if (appUser is null) return (false, null, "User Not Found");
            var result = await signInManager.CheckPasswordSignInAsync(appUser, password, lockoutOnFailure);
            if (!result.Succeeded)
            {
                var reason = result switch
                {
                    { IsLockedOut: true } => "Account is locked out.",
                    { IsNotAllowed: true } => "Account is not allowed to sign in.",
                    { RequiresTwoFactor: true } => "Two-factor authentication required.",
                    _ => "Invalid password."
                };

                logger.LogWarning("Password change failed for User ID: {UserId}. Because Of: {reason}", email, reason);
                return (false, null, reason);
            }
            var user = mapper.Map<AppUserDto>(appUser);
            return (true, user, string.Empty);
        }
        public async Task SignOutAsync(CancellationToken ct)
        {
            logger.LogDebug("Signing out current user session");
            await signInManager.SignOutAsync();
        }
    }
}
