using TransitNova.Domain.Enums.Users;
namespace TransitNova.BusinessLayer.Common.CommonData
{
    public class CommonRetrieveData
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public UserType UserType { get; set; }
        public string CityName { get; set; } = string.Empty;
        public string GovernmentName { get; set; } = string.Empty;
        public string CountryName { get; set; } = string.Empty;
    }
}
