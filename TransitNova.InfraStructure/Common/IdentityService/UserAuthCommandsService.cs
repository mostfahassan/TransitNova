using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.DTOs.AppUser;
using TransitNova.BusinessLayer.Interfaces.Services.IdentityOperationService;
using TransitNova.InfraStructure;
using TransitNova.InfraStructure.Common.Exceptions;
internal class UserAuthCommandsService(
    IMapper mapper,
    UserManager<AppUser> userManager,
    ILogger<UserAuthCommandsService> logger)
    : IUserAuthCommandsService
{
    public async Task<AppUserDto> CreateUserAsync(AppUserDto userDto, string password, CancellationToken ct)
    {
        logger.LogInformation("Creating user with Email: {Email}", userDto.Email);

        var appUser = mapper.Map<AppUser>(userDto);

        var result = await userManager.CreateAsync(appUser, password);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));

            logger.LogWarning("User creation failed for {Email}. Errors: {Errors}",
                userDto.Email,
                errors);

            throw new UserCreationException(errors);
        }
        return mapper.Map<AppUserDto>(appUser);
    }

    public async Task AddToRoleAsync(AppUserDto userDto, string roleName, CancellationToken ct)
    {
        logger.LogDebug("Adding User ID: {UserId} to Role: {RoleName}",
                userDto.Id, roleName);

        var appUser = await userManager.FindByIdAsync(userDto.Id.ToString());

       _ = appUser ?? throw new UserNotFoundException(userDto.Id);

        var result = await userManager.AddToRoleAsync(appUser, roleName);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));

            logger.LogWarning("Failed to add User ID: {UserId} to Role: {RoleName}",
                userDto.Id,
                roleName);

            throw new RoleAssignmentException(userDto.Id, roleName, errors);
        }
    }

    public async Task ChangePasswordAsync(string userId, string currentPassword, string newPassword, CancellationToken ct)
    {
        logger.LogDebug("Attempting password change for User ID: {UserId}", userId);

        var appUser = await userManager.FindByIdAsync(userId);

        _ = appUser ?? throw new UserNotFoundException(Guid.Parse(userId));
        var result = await userManager.ChangePasswordAsync(appUser, currentPassword, newPassword);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));

            throw new PasswordChangeException(appUser.Id, errors);
        }
    }

    public async Task DeleteUserAsync(AppUserDto userDto, CancellationToken ct)
    {
        var appUser = mapper.Map<AppUser>(userDto);

        var result = await userManager.DeleteAsync(appUser);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));

            throw new UserDeletionException(appUser.Id, errors);
        }
    }
}

