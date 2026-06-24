using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using TransitNova.BusinessLayer.Interfaces.Repositories.CarrierRepository;
namespace TransitNova.Api.AuthorizationResource.Requirement
{
    public sealed class CompletedProfileRequirement : IAuthorizationRequirement;

}
