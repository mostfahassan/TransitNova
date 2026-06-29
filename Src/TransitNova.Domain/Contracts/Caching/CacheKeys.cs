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
            public const string Dashboard = OperationManagersPrefix + ":dashboard";
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
            public static string Details(Guid tripId) => $"{TripsPrefix}:id:{tripId}";
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

        public static string BundleList() => Bundles.List;
        public static string BundleById(Guid id) => Bundles.ById(id);
        public static string BundleSubscriptionDetails(Guid subscriptionId) => Bundles.SubscriptionDetails(subscriptionId);
        public static string CarrierDashboard(Guid carrierId) => Carriers.Dashboard(carrierId);
        public static string CarrierProfile(Guid carrierId) => Carriers.Profile(carrierId);
        public static string CarrierRating(Guid carrierId) => Carriers.Rating(carrierId);
        public static string CarrierShipmentDetails(Guid carrierId, Guid shipmentId) => Carriers.ShipmentDetails(carrierId, shipmentId);
        public static string CarrierShipments(Guid carrierId, object filter) => Carriers.Shipments(carrierId, filter);
        public static string CarrierTripDetails(Guid carrierId, Guid tripId) => Carriers.TripDetails(carrierId, tripId);
        public static string CarrierTrips(Guid carrierId) => Carriers.Trips(carrierId);
        public static string CarrierFilter(object filterCriteria) => Carriers.Filter(filterCriteria);
        public static string CarriersByStatus(object carrierStatus) => Carriers.ByStatus(carrierStatus);
        public static string CarrierShipmentsByCarrier(Guid carrierId, Guid currentUser) => Carriers.ShipmentsByCarrier(carrierId, currentUser);
        public static string CityById(int id) => Cities.ById(id);
        public static string CitiesByCountry(int countryId) => Cities.ByCountry(countryId);
        public static string CitiesByGovernment(int governmentId) => Cities.ByGovernment(governmentId);
        public static string CityFilter(object filter) => Cities.Filter(filter);
        public static string CountryById(int id) => Countries.ById(id);
        public static string CountryList() => Countries.List;
        public static string CountryGovernments(int countryId) => Countries.Governments(countryId);
        public static string CountryFilter(object filter) => Countries.Filter(filter);
        public static string OperationManagerAssignedShipments(object filter) => OperationManagers.AssignedShipments(filter);
        public static string OperationManagerDashboard() => OperationManagers.Dashboard;
        public static string AdminDashboard() => Admins.Dashboard;
        public static string OperationManagerDetails(Guid operationManagerId) => OperationManagers.Details(operationManagerId);
        public static string OperationManagerHandledCarriers(Guid operationManagerId, int pageNumber, int pageSize) => OperationManagers.HandledCarriers(operationManagerId, pageNumber, pageSize);
        public static string OperationManagerHandledShipments(Guid operationManagerId, int pageNumber, int pageSize) => OperationManagers.HandledShipments(operationManagerId, pageNumber, pageSize);
        public static string OperationManagerShipmentHistories(Guid shipmentId) => OperationManagers.ShipmentHistories(shipmentId);
        public static string OperationManagerFilterCarriers(object filterCriteria) => OperationManagers.FilterCarriers(filterCriteria);
        public static string ShipmentById(Guid shipmentId) => Shipments.ById(shipmentId);
        public static string ShipmentByTrackingNumber(string trackingNumber) => Shipments.ByTrackingNumber(trackingNumber);
        public static string ShipmentFilter(object filterCriteria) => Shipments.Filter(filterCriteria);
        public static string ShipmentHistories(string trackingNumber) => Shipments.Histories(trackingNumber);
        public static string ShipmentStatistics() => Shipments.Statistics;
        public static string UserShipmentByTrackingNumber(string trackingNumber) => Shipments.UserByTrackingNumber(trackingNumber);
        public static string UserShipmentsInfo(Guid appUserId) => Shipments.UserShipmentsInfo(appUserId);
        public static string TripFilter(object filterCriteria) => Trips.Filter(filterCriteria);
        public static string TripDetails(Guid tripId) => Trips.Details(tripId);
        public static string AdminUserDetails(Guid userId) => Users.AdminDetails(userId);
        public static string UserDashboard(Guid appUserId) => Users.Dashboard(appUserId);
        public static string UserId(Guid appUserId) => Users.ByAppUserId(appUserId);
        public static string UserProfile(Guid appUserId) => Users.Profile(appUserId);
        public static string UserFilter(object filterCriteria) => Users.Filter(filterCriteria);
        public static string UserShipment(Guid appUserId, Guid shipmentId) => Users.Shipment(appUserId, shipmentId);
        public static string ActiveVehicles() => Vehicles.Active;
        public static string VehicleById(Guid id) => Vehicles.ById(id);
        public static string VehicleByPlateNumber(string plateNumber) => Vehicles.ByPlateNumber(plateNumber);
        public static string VehicleList() => Vehicles.List;
        public static string VehiclesByCarrierId(Guid carrierId) => Vehicles.ByCarrierId(carrierId);
        public static string ZoneById(Guid id) => Zones.ById(id);
        public static string ZonesByCity(int cityId, object filter) => Zones.ByCity(cityId, filter);
        public static string ZoneFilter(object filter) => Zones.Filter(filter);

        private static string Serialize(object? value)
            => Escape(JsonSerializer.Serialize(value));

        private static string Escape(string? value)
            => Uri.EscapeDataString(value ?? string.Empty);
    }
}
