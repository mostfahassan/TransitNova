namespace TransitNova.Domain.Contracts.Permissions
{
    public static class OperationManagerPermissions
    {

        // Dashboard
        public const string ViewDashboard = "OperationManager.CanViewDashboard";
        public const string ViewStatistics = "OperationManager.ViewStatistics";
        public const string ViewReports = "OperationManager.ViewReports";
        public const string GenerateReports = "OperationManager.GenerateReports";

        // Shipments
        public const string ApproveShipment = "OperationManager.ApproveShipment";
        public const string RejectShipment = "OperationManager.RejectShipment";
        public const string ViewShipmentDetails = "OperationManager.ViewShipmentDetails";
        public const string ViewPendingShipments = "OperationManager.ViewPendingShipments";
        public const string ViewAssignedShipments = "OperationManager.ViewAssignedShipments";
        public const string ViewRejectedShipments = "OperationManager.ViewRejectedShipments";
        public const string ViewAllShipments = "OperationManager.ViewAllShipments";
        public const string TrackShipment = "OperationManager.TrackShipment";
        public const string ViewShipmentHistory = "OperationManager.ViewShipmentHistory";

        // Carrier Management
        public const string ViewCarriers = "OperationManager.ViewCarriers";
        public const string ViewCarrierDetails = "OperationManager.ViewCarrierDetails";
        public const string CheckCarrierAvailability = "OperationManager.CheckCarrierAvailability";
        public const string AssignPickupCarrier = "OperationManager.AssignPickupCarrier";
        public const string AssignDeliveryCarrier = "OperationManager.AssignDeliveryCarrier";

        // Trip Management
        public const string ViewTrips = "OperationManager.ViewTrips";
        public const string ViewTripDetails = "OperationManager.ViewTripDetails";
        public const string CreateTrip = "OperationManager.CreateTrip";
        public const string AssignShipmentToTrip = "OperationManager.AssignShipmentToTrip";
        public const string StartPickupTrip = "OperationManager.StartPickupTrip";
        public const string StartDeliveryTrip = "OperationManager.StartDeliveryTrip";

        // Warehouses
        public const string ViewWarehouses = "OperationManager.ViewWarehouses";
        public const string ViewWarehouseDetails = "OperationManager.ViewWarehouseDetails";

        // Profile
        public const string ViewProfile = "OperationManager.ViewProfile";
        public const string UpdateProfile = "OperationManager.UpdateProfile";

        public static IReadOnlyList<string> All =>
             [
                ViewDashboard,
                ViewStatistics,
                ViewReports,
                ApproveShipment,
                RejectShipment,
                ViewShipmentDetails,
                ViewPendingShipments,
                ViewAssignedShipments,
                ViewRejectedShipments,
                ViewAllShipments,
                TrackShipment,
                ViewShipmentHistory,
                ViewCarriers,
                ViewCarrierDetails,
                CheckCarrierAvailability,
                AssignPickupCarrier,
                AssignDeliveryCarrier,
                ViewTrips,
                ViewTripDetails,
                CreateTrip,
                AssignShipmentToTrip,
                StartPickupTrip,
                StartDeliveryTrip,
                ViewWarehouses,
                ViewWarehouseDetails,
                ViewProfile,
                UpdateProfile
            ];
    }
}
