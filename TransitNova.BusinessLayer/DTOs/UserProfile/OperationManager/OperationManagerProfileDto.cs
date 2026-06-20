
using TransitNova.BusinessLayer.Common.CommonData;

namespace TransitNova.BusinessLayer.DTOs.UserProfile.OperationManager
{
    public class OperationManagerProfileDto: CommonRetrieveData
    {
        public int TotalShipmentHandled { get; set; }
        public int TotalCarriertHandled { get; set; }
    }
}
