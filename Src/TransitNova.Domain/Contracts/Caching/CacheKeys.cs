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
        public static string BundleList() => $"{BundlesPrefix}:list";
        public static string BundleById(Guid id) => $"{BundlesPrefix}:id:{id}";
        public static string BundleSubscriptionDetails(Guid subscriptionId) => $"{BundlesPrefix}:subscription-id:{subscriptionId}";
        public static string CarrierDashboard(Guid carrierId) => $"{CarriersPrefix}:dashboard:carrier-id:{carrierId}";
        public static string CarrierProfile(Guid carrierId) => $"{CarriersPrefix}:profile:carrier-id:{carrierId}";
        public static string CarrierRating(Guid carrierId) => $"{CarriersPrefix}:rating:carrier-id:{carrierId}";
        public static string CarrierShipmentDetails(Guid carrierId, Guid shipmentId) => $"{CarriersPrefix}:shipment-details:carrier-id:{carrierId}:shipment-id:{shipmentId}";
        public static string CarrierShipments(Guid carrierId, object filter) => $"{CarriersPrefix}:shipments:carrier-id:{carrierId}:filter:{Serialize(filter)}";
        public static string CarrierTripDetails(Guid carrierId, Guid tripId) => $"{CarriersPrefix}:trip-details:carrier-id:{carrierId}:trip-id:{tripId}";
        public static string CarrierTrips(Guid carrierId) => $"{CarriersPrefix}:trips:carrier-id:{carrierId}";
        public static string CarrierFilter(object filterCriteria) => $"{CarriersPrefix}:filter:{Serialize(filterCriteria)}";
        public static string CarriersByStatus(object carrierStatus) => $"{CarriersPrefix}:status:{Serialize(carrierStatus)}";
        public static string CarrierShipmentsByCarrier(Guid carrierId, Guid currentUser) => $"{CarriersPrefix}:shipments-by-carrier:carrier-id:{carrierId}:current-user:{currentUser}";

        public static string CityById(int id) => $"{CitiesPrefix}:id:{id}";
        public static string CitiesByCountry(int countryId) => $"{CitiesPrefix}:country-id:{countryId}";
        public static string CitiesByGovernment(int governmentId) => $"{CitiesPrefix}:government-id:{governmentId}";
        public static string CityFilter(object filter) => $"{CitiesPrefix}:filter:{Serialize(filter)}";

        public static string CountryById(int id) => $"{CountriesPrefix}:id:{id}";
        public static string CountryList() => $"{CountriesPrefix}:list";
        public static string CountryGovernments(int countryId) => $"{CountriesPrefix}:governments:country-id:{countryId}";
        public static string CountryFilter(object filter) => $"{CountriesPrefix}:filter:{Serialize(filter)}";

        public static string OperationManagerAssignedShipments(object filter) => $"{OperationManagersPrefix}:assigned-shipments:{Serialize(filter)}";
        public static string OperationManagerDashboard() => $"{OperationManagersPrefix}:dashboard";
        public static string AdminDashboard() => $"{AdminsPrefix}:dashboard";
        public static string OperationManagerDetails(Guid operationManagerId) => $"{OperationManagersPrefix}:id:{operationManagerId}";
        public static string OperationManagerHandledCarriers(Guid operationManagerId, int pageNumber, int pageSize) => $"{OperationManagersPrefix}:handled-carriers:operation-manager-id:{operationManagerId}:page-number:{pageNumber}:page-size:{pageSize}";
        public static string OperationManagerHandledShipments(Guid operationManagerId, int pageNumber, int pageSize) => $"{OperationManagersPrefix}:handled-shipments:operation-manager-id:{operationManagerId}:page-number:{pageNumber}:page-size:{pageSize}";
        public static string OperationManagerShipmentHistories(Guid shipmentId) => $"{OperationManagersPrefix}:shipment-histories:shipment-id:{shipmentId}";
        public static string OperationManagerFilterCarriers(object filterCriteria) => $"{OperationManagersPrefix}:carriers:filter:{Serialize(filterCriteria)}";

        public static string ShipmentById(Guid shipmentId) => $"{ShipmentsPrefix}:id:{shipmentId}";
        public static string ShipmentByTrackingNumber(string trackingNumber) => $"{ShipmentsPrefix}:tracking-number:{Escape(trackingNumber)}";
        public static string ShipmentFilter(object filterCriteria) => $"{ShipmentsPrefix}:filter:{Serialize(filterCriteria)}";
        public static string ShipmentHistories(string trackingNumber) => $"{ShipmentsPrefix}:histories:tracking-number:{Escape(trackingNumber)}";
        public static string ShipmentStatistics() => $"{ShipmentsPrefix}:statistics";
        public static string UserShipmentByTrackingNumber(string trackingNumber) => $"{ShipmentsPrefix}:user:tracking-number:{Escape(trackingNumber)}";
        public static string UserShipmentsInfo(Guid appUserId) => $"{ShipmentsPrefix}:user-shipments-info:app-user-id:{appUserId}";

        public static string TripFilter(object filterCriteria) => $"{TripsPrefix}:filter:{Serialize(filterCriteria)}";

        public static string AdminUserDetails(Guid userId) => $"{UsersPrefix}:admin-details:user-id:{userId}";
        public static string UserDashboard(Guid appUserId) => $"{UsersPrefix}:dashboard:app-user-id:{appUserId}";
        public static string UserId(Guid appUserId) => $"{UsersPrefix}:app-user-id:{appUserId}";
        public static string UserProfile(Guid appUserId) => $"{UsersPrefix}:profile:app-user-id:{appUserId}";
        public static string UserFilter(object filterCriteria) => $"{UsersPrefix}:filter:{Serialize(filterCriteria)}";
        public static string UserShipment(Guid appUserId, Guid shipmentId) => $"{UsersPrefix}:shipment:app-user-id:{appUserId}:shipment-id:{shipmentId}";

        public static string ActiveVehicles() => $"{VehiclesPrefix}:active";
        public static string VehicleById(Guid id) => $"{VehiclesPrefix}:id:{id}";
        public static string VehicleByPlateNumber(string plateNumber) => $"{VehiclesPrefix}:plate-number:{Escape(plateNumber)}";
        public static string VehicleList() => $"{VehiclesPrefix}:list";
        public static string VehiclesByCarrierId(Guid carrierId) => $"{VehiclesPrefix}:carrier-id:{carrierId}";

        public static string ZoneById(Guid id) => $"{ZonesPrefix}:id:{id}";
        public static string ZonesByCity(int cityId, object filter) => $"{ZonesPrefix}:city-id:{cityId}:filter:{Serialize(filter)}";
        public static string ZoneFilter(object filter) => $"{ZonesPrefix}:filter:{Serialize(filter)}";

        private static string Serialize(object? value)
            => Escape(JsonSerializer.Serialize(value));

        private static string Escape(string? value)
            => Uri.EscapeDataString(value ?? string.Empty);
    }
}
