
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using TransitNova.BusinessLayer.DTOs.AppUser;
using TransitNova.BusinessLayer.Interfaces.Token;
using TransitNova.Domain.Contracts.Permissions;
using TransitNova.Domain.Contracts.Roles;
namespace TransitNova.InfraStructure.Token
{
    public class TokenGenerator(IOptions<JwtSettings>Jwt) : ITokenProvider
    {
        public string GenerateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        }
        public async Task<string> GenerateTokenAsync(AppUserDto user)
        {

            var jwt = Jwt.Value;
            var claims = new List<Claim>
             {
                new (ClaimTypes.NameIdentifier, user.Id.ToString()),
                new (ClaimTypes.Email, user.Email),
                new (ClaimTypes.Name, user.UserName),
                new (ClaimTypes.MobilePhone, user.PhoneNumber!),
                new ("user_Type" , user.UserType.ToString()),
                new (Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
             };
           
            var roles = user.Roles;
            var permissions = new HashSet<string>();
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));

                switch (role)
                {
                    case Role.User:
                        permissions.UnionWith(UserPermissions.All);
                        break;

                    case Role.Carrier:
                        permissions.UnionWith(CarrierPermissions.All);
                        break;

                    case Role.OperationManager:
                        permissions.UnionWith(OperationManagerPermissions.All);
                        break;

                    case Role.WarehouseManager:
                        permissions.UnionWith(WarehouseManagerPermissions.All);
                        break;

                    case Role.Admin:
                        permissions.UnionWith(AdminPermissions.All);
                        break;
                }
            }

            claims.AddRange(permissions.Select(p => new Claim("Permission", p)));
            
            
            if (string.IsNullOrWhiteSpace(jwt.Key))
                throw new InvalidOperationException("JWT key is not configured.");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key ?? string.Empty));
          
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha384Signature);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(1),
                Issuer = jwt.Issuer,
                Audience = jwt.Audience,
                SigningCredentials = creds
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }


}
