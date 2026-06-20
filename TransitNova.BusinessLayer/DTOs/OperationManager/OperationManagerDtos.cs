using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Shipment;
using TransitNova.Domain.Enums.Carrier;
using TransitNova.Domain.Enums.Shipment;
using TransitNova.Domain.Enums.Users;

namespace TransitNova.BusinessLayer.DTOs.OperationManager
{
    public class OperationManagerDashboardDto
    {
        public int TotalShipments { get; set; }
        public int PendingShipments { get; set; }
        public int DeliveredShipments { get; set; }
        public int ActiveShipments { get; set; }
        public IReadOnlyCollection<OperationManagerStatusStatDto> ShipmentStatistics { get; set; } = [];
        public IReadOnlyCollection<OperationManagerActivityDto> RecentActivity { get; set; } = [];
        public IReadOnlyCollection<RetrieveShipmentDto> RecentShipments { get; set; } = [];
        public IReadOnlyCollection <RetrieveCarriersForOperationManagerDto> RecentCarriers { get; set; } = [];
    }
    public class ProfileDashboardDto
    {
        public int TotalShipments { get; set; }
        public int PendingShipments { get; set; }
        public int DeliveredShipments { get; set; }
        public int ActiveShipments { get; set; }
        public int IssueShipments { get; set; }
        public IReadOnlyCollection<RetrieveShipmentSummaryDto> ShipmentSummary { get; set; } = [];
    }

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

    public class OperationManagerStatusStatDto
    {
        public ShipmentStatuses Status { get; set; }
        public int Count { get; set; }
    }

    public class OperationManagerActivityDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime OccurredAt { get; set; }
        public ShipmentStatuses Status { get; set; }
    }

    public class OperationManagerShipmentStatusUpdateDto
    {
        public ShipmentStatuses Status { get; set; }
        public string? RejectionReason { get; set; }
    }

    public class OperationManagerShipmentListDto
    {
        public PagedResult<RetrieveShipmentDto> Shipments { get; set; } = PagedResult<RetrieveShipmentDto>.From([], 0, 1, 20);
    }


    //public class OperationManagerProfileDto
    //{
    //    public string FirstName { get;  set; } = string.Empty;
    //    public string LastName { get;  set; } = string.Empty;
    //    public string FullName => $"{FirstName} {LastName}".Trim();
    //    public string Email { get;  set; } = string.Empty;
    //    public string PhoneNumber { get;  set; } = string.Empty;
    //    public string Address { get;  set; } = string.Empty;
    //    public UserType UserType { get; set;}
    //    public int TotalShipmentHandled { get; set; }
    //    public int TotalCarriertHandled { get; set; }
    //}
}


