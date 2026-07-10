using System.Text.Json;

namespace TransitNova.Domain.Contracts.Caching
{
    public static class CacheKeys
    {
        public static readonly TimeSpan DefaultExpiration = TimeSpan.FromMinutes(20);

        public const string BundlesPrefix = "bundles";
        public const string CarrierCompaniesPrefix = "carrier-companies";
        public const string CarriersPrefix = "carriers";
        public const string CitiesPrefix = "cities";
        public const string CountriesPrefix = "countries";
        public const string OperationManagersPrefix = "operation-managers";
        public const string AdminsPrefix = "admins";
        public const string ShipmentsPrefix = "shipments";
        public const string TripsPrefix = "trips";
        public const string UsersPrefix = "users";
        public const string VehiclesPrefix = "vehicles";
        public const string ZonesPrefix = "zones";
        public const string RolesPrefix = "roles";
        public const string WarehouseManagersPrefix = "warehouse-managers";
        public const string WarehousesPrefix = "warehouses";

        public static class Bundles
        {
            public const string List = BundlesPrefix + ":list";
            public static string ById(Guid id) => $"{BundlesPrefix}:id:{id}";
            public static string SubscriptionDetails(Guid subscriptionId) => $"{BundlesPrefix}:subscription-id:{subscriptionId}";
        }

        public static class Carriers
        {
            public static string Dashboard(Guid carrierId) => $"{CarriersPrefix}:dashboard:carrier-id:{carrierId}";
            public static string Profile(Guid carrierId) => $"{CarriersPrefix}:profile:carrier-id:{carrierId}";
            public static string Rating(Guid carrierId) => $"{CarriersPrefix}:rating:carrier-id:{carrierId}";
            public static string ShipmentDetails(Guid carrierId, Guid shipmentId) => $"{CarriersPrefix}:shipment-details:carrier-id:{carrierId}:shipment-id:{shipmentId}";
            public static string Shipments(Guid carrierId, object filter) => $"{CarriersPrefix}:shipments:carrier-id:{carrierId}:filter:{Serialize(filter)}";
            public static string TripDetails(Guid carrierId, Guid tripId) => $"{CarriersPrefix}:trip-details:carrier-id:{carrierId}:trip-id:{tripId}";
            public static string Trips(Guid carrierId) => $"{CarriersPrefix}:trips:carrier-id:{carrierId}";
            public static string Filter(object filterCriteria) => $"{CarriersPrefix}:filter:{Serialize(filterCriteria)}";
            public static string ByStatus(object carrierStatus) => $"{CarriersPrefix}:status:{Serialize(carrierStatus)}";
            public static string ShipmentsByCarrier(Guid carrierId, Guid currentUser) => $"{CarriersPrefix}:shipments-by-carrier:carrier-id:{carrierId}:current-user:{currentUser}";
            public static string Revenue(Guid carrierId) => $"{CarriersPrefix}:revenue:carrier-id:{carrierId}";
        }

        public static class Cities
        {
            public static string ById(int id) => $"{CitiesPrefix}:id:{id}";
            public static string ByCountry(int countryId) => $"{CitiesPrefix}:country-id:{countryId}";
            public static string ByGovernment(int governmentId) => $"{CitiesPrefix}:government-id:{governmentId}";
            public static string Filter(object filter) => $"{CitiesPrefix}:filter:{Serialize(filter)}";
        }

        public static class Countries
        {
            public const string List = CountriesPrefix + ":list";
            public static string ById(int id) => $"{CountriesPrefix}:id:{id}";
            public static string Governments(int countryId) => $"{CountriesPrefix}:governments:country-id:{countryId}";
            public static string Filter(object filter) => $"{CountriesPrefix}:filter:{Serialize(filter)}";
        }

        public static class OperationManagers
        {
            public static string Dashboard(Guid operationManagerId) => $"{OperationManagersPrefix}:dashboard:id:{operationManagerId}";

            public const string OperationManagersDashboard = $"{OperationManagersPrefix}:dashboard";
            public const string ActiveList = $"{OperationManagersPrefix}:active-list";
            public static string AssignedShipments(object filter) => $"{OperationManagersPrefix}:assigned-shipments:{Serialize(filter)}";
            public static string Details(Guid operationManagerId) => $"{OperationManagersPrefix}:id:{operationManagerId}";
            public static string HandledCarriers(Guid operationManagerId, int pageNumber, int pageSize) => $"{OperationManagersPrefix}:handled-carriers:operation-manager-id:{operationManagerId}:page-number:{pageNumber}:page-size:{pageSize}";
            public static string HandledShipments(Guid operationManagerId, int pageNumber, int pageSize) => $"{OperationManagersPrefix}:handled-shipments:operation-manager-id:{operationManagerId}:page-number:{pageNumber}:page-size:{pageSize}";
            public static string ShipmentHistories(Guid shipmentId) => $"{OperationManagersPrefix}:shipment-histories:shipment-id:{shipmentId}";
            public static string FilterCarriers(object filterCriteria) => $"{OperationManagersPrefix}:carriers:filter:{Serialize(filterCriteria)}";
        }

        public static class Admins
        {
            public const string Dashboard = AdminsPrefix + ":dashboard";
            public const string Managers = AdminsPrefix + ":managers";
            public static string Subscribers(Guid bundleId) => $"{AdminsPrefix}:subscribers:bundle-id:{bundleId}";
        }

        public static class Roles
        {
            public static string ById(Guid roleId) => $"{RolesPrefix}:id:{roleId}";
            public const string List = RolesPrefix + ":list";
            public const string Member = RolesPrefix + ":member";
            public const string MemberList = RolesPrefix + ":member-list";
        }

        public static class Shipments
        {
            public const string Statistics = ShipmentsPrefix + ":statistics";
            public static string ById(Guid shipmentId) => $"{ShipmentsPrefix}:id:{shipmentId}";
            public static string ByTrackingNumber(string trackingNumber) => $"{ShipmentsPrefix}:tracking-number:{Escape(trackingNumber)}";
            public static string Filter(object filterCriteria) => $"{ShipmentsPrefix}:filter:{Serialize(filterCriteria)}";
            public static string Histories(string trackingNumber) => $"{ShipmentsPrefix}:histories:tracking-number:{Escape(trackingNumber)}";
            public static string UserByTrackingNumber(string trackingNumber) => $"{ShipmentsPrefix}:user:tracking-number:{Escape(trackingNumber)}";
            public static string UserShipmentsInfo(Guid appUserId) => $"{ShipmentsPrefix}:user-shipments-info:app-user-id:{appUserId}";
        }

        public static class Trips
        {
            public static string Filter(object filterCriteria) => $"{TripsPrefix}:filter:{Serialize(filterCriteria)}";
            public static string Details(Guid tripId, Guid? handlerId = null) =>
                handlerId.HasValue
                    ? $"{TripsPrefix}:id:{tripId}:handler-id:{handlerId.Value}"
                    : $"{TripsPrefix}:id:{tripId}";
        }

        public static class WarehouseManagers
        {
            public static string Filter(object filterCriteria) => $"{WarehouseManagersPrefix}:filter:{Serialize(filterCriteria)}";
            public static string Details(Guid warehouseManagerId) => $"{WarehouseManagersPrefix}:id:{warehouseManagerId}";
            public static string Dashboard(Guid warehouseManagerId) => $"{WarehouseManagersPrefix}:dashboard:warehouse-manager-id:{warehouseManagerId}";
        }

        public static class Warehouse
        {
            public static string Filter(object filterCriteria) => $"{WarehousesPrefix}:filter:{Serialize(filterCriteria)}";
            public static string ById(Guid warehouseId) => $"{WarehousesPrefix}:id:{warehouseId}";
            public static string Dashboard(Guid warehouseId) => $"{WarehousesPrefix}:dashboard:warehouse-id:{warehouseId}";
            public const string List = WarehousesPrefix + ":list";
        }

        public static class Users
        {
            public static string AdminDetails(Guid userId) => $"{UsersPrefix}:admin-details:user-id:{userId}";
            public static string Dashboard(Guid appUserId) => $"{UsersPrefix}:dashboard:app-user-id:{appUserId}";
            public static string ByAppUserId(Guid appUserId) => $"{UsersPrefix}:app-user-id:{appUserId}";
            public static string Profile(Guid appUserId) => $"{UsersPrefix}:profile:app-user-id:{appUserId}";
            public static string Filter(object filterCriteria) => $"{UsersPrefix}:filter:{Serialize(filterCriteria)}";
            public static string Shipment(Guid appUserId, Guid shipmentId) => $"{UsersPrefix}:shipment:app-user-id:{appUserId}:shipment-id:{shipmentId}";
        }

        public static class Vehicles
        {
            public const string Active = VehiclesPrefix + ":active";
            public const string List = VehiclesPrefix + ":list";
            public static string ById(Guid id) => $"{VehiclesPrefix}:id:{id}";
            public static string ByPlateNumber(string plateNumber) => $"{VehiclesPrefix}:plate-number:{Escape(plateNumber)}";
            public static string ByCarrierId(Guid carrierId) => $"{VehiclesPrefix}:carrier-id:{carrierId}";
        }

        public static class Zones
        {
            public static string ById(Guid id) => $"{ZonesPrefix}:id:{id}";
            public static string ByCity(int cityId, object filter) => $"{ZonesPrefix}:city-id:{cityId}:filter:{Serialize(filter)}";
            public static string Filter(object filter) => $"{ZonesPrefix}:filter:{Serialize(filter)}";
        }

        private static string Serialize(object? value)
            => Escape(JsonSerializer.Serialize(value));

        private static string Escape(string? value)
            => Uri.EscapeDataString(value ?? string.Empty);
    }
}