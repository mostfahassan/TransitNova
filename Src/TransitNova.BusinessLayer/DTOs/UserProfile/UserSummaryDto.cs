using TransitNova.BusinessLayer.Common.CommonData;

namespace TransitNova.BusinessLayer.DTOs.UserProfile
{
    public class UserSummaryDto
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public AddressDto Address { get; set; } = new();
        public string CityName { get; set; } = string.Empty;
    }
}
