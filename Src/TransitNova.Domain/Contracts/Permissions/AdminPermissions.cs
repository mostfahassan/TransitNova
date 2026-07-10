namespace TransitNova.Domain.Contracts.Permissions
{
    public static class AdminPermissions
    {
        // Dashboard & Reports

        public const string ViewDashboard = "Admin.ViewDashboard";
        public const string ViewStatistics = "Admin.ViewStatistics";
        public const string ViewReports = "Admin.ViewReports";
        public const string ViewPaymentHistories = "Admin.ViewPaymentHistories";

        // User Management

        public const string ViewUsers = "Admin.ViewUsers";
        public const string ViewUserDetails = "Admin.ViewUserDetails";
        public const string SuspendUser = "Admin.SuspendUser";
        public const string ActivateUser = "Admin.ActivateUser";
        public const string DeleteUser = "Admin.DeleteUser";

        // Carrier Management

        public const string ViewCarriers = "Admin.ViewCarriers";
        public const string ViewCarrierDetails = "Admin.ViewCarrierDetails";
        public const string ApproveCarrier = "Admin.ApproveCarrier";
        public const string RejectCarrier = "Admin.RejectCarrier";
        public const string SuspendCarrier = "Admin.SuspendCarrier";
        public const string ActivateCarrier = "Admin.ActivateCarrier";
        public const string DeleteCarrier = "Admin.DeleteCarrier";

        // Operation Manager Management

        public const string ViewOperationManagers = "Admin.ViewOperationManagers";
        public const string ViewOperationManagerDetails = "Admin.ViewOperationManagerDetails";
        public const string CreateOperationManager = "Admin.CreateOperationManager";
        public const string UpdateOperationManager = "Admin.UpdateOperationManager";
        public const string DeleteOperationManager = "Admin.DeleteOperationManager";

        // Shipment Management

        public const string ViewShipments = "Admin.ViewShipments";
        public const string ViewShipmentDetails = "Admin.ViewShipmentDetails";
        public const string TrackShipment = "Admin.TrackShipment";

        // Trip Management

        public const string ViewTrips = "Admin.ViewTrips";
        public const string ViewTripDetails = "Admin.ViewTripDetails";

        // Warehouse Management

        public const string ViewWarehouses = "Admin.ViewWarehouses";
        public const string ViewWarehouseDetails = "Admin.ViewWarehouseDetails";
        public const string CreateWarehouse = "Admin.CreateWarehouse";
        public const string UpdateWarehouse = "Admin.UpdateWarehouse";
        public const string DeleteWarehouse = "Admin.DeleteWarehouse";

        // Vehicle Management

        public const string ViewVehicles = "Admin.ViewVehicles";
        public const string ViewVehicleDetails = "Admin.ViewVehicleDetails";
        public const string CreateVehicle = "Admin.CreateVehicle";
        public const string UpdateVehicle = "Admin.UpdateVehicle";
        public const string DeleteVehicle = "Admin.DeleteVehicle";

        // Bundle Management

        public const string ViewBundles = "Admin.ViewBundles";
        public const string ViewBundleDetails = "Admin.ViewBundleDetails";
        public const string CreateBundle = "Admin.CreateBundle";
        public const string UpdateBundle = "Admin.UpdateBundle";
        public const string DeleteBundle = "Admin.DeleteBundle";

        // Profile

        public const string ViewProfile = "Admin.ViewProfile";
        public const string UpdateProfile = "Admin.UpdateProfile";

        // Role Management

        public const string ViewRoles = "Admin.ViewRoles";
        public const string ViewRoleDetails = "Admin.ViewRoleDetails";
        public const string CreateRole = "Admin.CreateRole";
        public const string UpdateRole = "Admin.UpdateRole";
        public const string DeleteRole = "Admin.DeleteRole";
        public const string ManageRoleMembers = "Admin.ManageRoleMembers";

        public static readonly string[] All =
        [
            // Dashboard

            ViewDashboard,
            ViewStatistics,
            ViewReports,
            ViewPaymentHistories,

            // Users

            ViewUsers,
            ViewUserDetails,
            SuspendUser,
            ActivateUser,
            DeleteUser,

            // Carriers

            ViewCarriers,
            ViewCarrierDetails,
            ApproveCarrier,
            RejectCarrier,
            SuspendCarrier,
            ActivateCarrier,
            DeleteCarrier,

            // Operation Managers

            ViewOperationManagers,
            ViewOperationManagerDetails,
            CreateOperationManager,
            UpdateOperationManager,
            DeleteOperationManager,

            // Shipments

            ViewShipments,
            ViewShipmentDetails,
            TrackShipment,

            // Trips

            ViewTrips,
            ViewTripDetails,

            // Warehouses

            ViewWarehouses,
            ViewWarehouseDetails,
            CreateWarehouse,
            UpdateWarehouse,
            DeleteWarehouse,

            // Vehicles

            ViewVehicles,
            ViewVehicleDetails,
            CreateVehicle,
            UpdateVehicle,
            DeleteVehicle,

            // Bundles

            ViewBundles,
            ViewBundleDetails,
            CreateBundle,
            UpdateBundle,
            DeleteBundle,

            // Profile

            ViewProfile,
            UpdateProfile,

            // Roles

            ViewRoles,
            ViewRoleDetails,
            CreateRole,
            UpdateRole,
            DeleteRole,
            ManageRoleMembers
        ];
    }
}
