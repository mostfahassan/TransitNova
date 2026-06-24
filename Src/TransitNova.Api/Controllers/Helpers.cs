
using System.Security.Claims;
namespace TransitNova.Api.Controllers
{
    public static class Helpers
    {
        public static Guid GetUserId(this ClaimsPrincipal user)
        {
            var value = user.FindFirstValue(ClaimTypes.NameIdentifier);

            return Guid.TryParse(value, out var id)
                ? id
                : Guid.Empty;
        }
    }

}

