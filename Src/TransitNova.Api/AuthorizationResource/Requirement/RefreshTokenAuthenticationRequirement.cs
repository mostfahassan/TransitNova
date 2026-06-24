using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using TransitNova.BusinessLayer.Interfaces.Repositories.TokenRepository;
namespace TransitNova.Api.AuthorizationResource.Requirement
{
    public class RefreshTokenAuthenticationRequirement : IAuthorizationRequirement
    {
    }
    public class RefreshTokenOwnershipRequirement : IAuthorizationRequirement
    {
    }

}
