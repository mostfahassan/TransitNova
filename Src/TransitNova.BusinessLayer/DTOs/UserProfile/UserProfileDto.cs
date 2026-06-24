
using TransitNova.BusinessLayer.Common.CommonData;

namespace TransitNova.BusinessLayer.DTOs.UserProfile
{
    public class UserProfileDto:CommonRetrieveData
    {
        public int TotalShipmentsSent { get; set; } = default;
        public string? BundleName { get; set; }
    }
}
