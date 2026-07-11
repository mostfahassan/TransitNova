namespace TransitNova.BusinessLayer.DTOs.Admin
{

    public sealed class AdminOperationalHealthDto
    {
        public int AvailableCarriers { get; set; }

        public int BusyCarriers { get; set; }

        public int ActiveOperationManagers { get; set; }

        public decimal AverageCarrierRating { get; set; }

        public decimal DeliverySuccessRate { get; set; }

        public decimal CancelledShipmentRate { get; set; }
    }
    public sealed class CarrierStats
    {
        public int TotalCarriers { get; set; }
        public int AvailableCarriers { get; set; }
        public int ActiveCarriers { get; set; }
        public int BusyCarriers { get; set; }
        public decimal DeliverySuccessRate { get; set; }
        public decimal AverageCarrierRating { get; set; }
    }
    public sealed class ShipmentStats
    {
        public int TotalShipments { get; set; }
        public decimal CancelledShipmentRate { get; set; }
        public int ActiveShipments { get; set; }
        public int PendingShipments { get; set; }
        public int DeliveredShipments { get; set; }
    }
    public sealed class OperationManagerStats
    {
        public int TotalOperationManagers { get; set; }
        public int ActiveOperationManagers { get; set; }
    }
    public sealed class TripsStatistics
    {
        public int TotalTrips { get; set; }
        public int PlannedTrips { get; set; }
        public int CompletedTrips { get; set; } 
        public int ActiveTrips { get; set; }
    }
    public sealed class UserStatistics
    {
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
    }
}
