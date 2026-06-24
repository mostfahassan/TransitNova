using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Shipment;
using TransitNova.Domain.Enums.Carrier;
using TransitNova.Domain.Enums.Shipment;
using TransitNova.Domain.Enums.Users;
namespace TransitNova.BusinessLayer.DTOs.OperationManager
{

    public class RetrieveCarriersForOperationManagerDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public CarrierStatus Status { get; set; }
        public int AssignedShipmentsCount { get; set; }
        public int ActiveTripsCount { get; set; }
        public List<string> ServedCities { get; set; } = [];
        public decimal Rating { get; set; }
    }

}
