using TransitNova.BusinessLayer.Common.CommonData;
using TransitNova.Domain.Enums.Users;
namespace TransitNova.BusinessLayer.DTOs.UserProfile.Auth
{
    public class RegisterDto
    {
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public AddressDto Address { get; set; } = new();
        public UserType UserType { get; set; }
        public int CityId { get; set; }

    }
}
