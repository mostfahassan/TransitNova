
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.DTOs.AppUser;
using TransitNova.BusinessLayer.Interfaces.Services.IdentityOperationService;
namespace TransitNova.InfraStructure.Common.IdentityService
{
    internal class UserAuthQueryService(IMapper mapper ,
        UserManager<AppUser> userManager ) : IUserAuthQueryService
    {
        public async Task<AppUserDto?> FindByIdAsync(Guid userId, CancellationToken ct)
        {
            var user = await userManager.FindByIdAsync(userId.ToString());
            return user is null ? null : mapper.Map<AppUserDto>(user);
        }

        public async Task<AppUserDto?> FindByNameAsync(string userName, CancellationToken ct)
        {
            var user = await userManager.FindByNameAsync(userName);
            return user is null ? null : mapper.Map<AppUserDto>(user);
        }

        public async Task<AppUserDto?> FindByEmailAsync(string email, CancellationToken ct)
        {
          
            var user = await userManager.FindByEmailAsync(email);
            return user is null ? null : mapper.Map<AppUserDto>(user);
        }
        public async Task<IList<string>> GetUserRolesAsync(Guid userId, CancellationToken ct)
        {
            var appUser = await userManager.FindByIdAsync(userId.ToString());
            return appUser is null ? []
            : await userManager.GetRolesAsync(appUser);
        }

      

     
    }
}
