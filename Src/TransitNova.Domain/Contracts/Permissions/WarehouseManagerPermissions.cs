namespace TransitNova.Domain.Contracts.Permissions
{
    public static class WarehouseManagerPermissions
    {
        public const string ViewDashboard = "WarehouseManagers.ViewDashboard";
        public const string ViewShipments = "WarehouseManagers.ViewShipments";
        public const string ViewShipmentDetails = "WarehouseManagers.ViewShipmentDetails";
        public const string ViewCarriers = "WarehouseManagers.ViewCarriers";
        public const string ViewCarrierDetails = "WarehouseManagers.ViewCarrierDetails";
        public const string ViewTrips = "WarehouseManagers.ViewTrips";
        public const string ViewTripDetails = "WarehouseManagers.ViewTripDetails";
        public const string Update = "WarehouseManagers.Update";
        public const string IsWarehouseManager = "WarehouseManagers.IsWarehouseManager";

        public static IReadOnlyList<string> All =>
        [
            ViewDashboard,
            ViewShipments,
            ViewCarriers,
            ViewCarrierDetails,
            ViewShipmentDetails,
            ViewTrips,
            ViewTripDetails,
            IsWarehouseManager,
            Update
        ];
    }
}
