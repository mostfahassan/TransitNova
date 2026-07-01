namespace TransitNovaUI.BusinessLayer.ApiContracts;

public static class ApiRoutes
{
    public const string Version = "1";
    public const string Prefix = $"api/v{Version}";

    public static string Build(string route, params (string Name, object? Value)[] values)
    {
        var resolvedRoute = route.TrimStart('/');
        var queryValues = new List<string>();

        foreach (var (name, value) in values)
        {
            if (value is null)
                continue;

            var placeholder = $"{{{name}}}";
            var formattedValue = Uri.EscapeDataString(value.ToString() ?? string.Empty);

            if (resolvedRoute.Contains(placeholder, StringComparison.OrdinalIgnoreCase))
            {
                resolvedRoute = resolvedRoute.Replace(placeholder, formattedValue, StringComparison.OrdinalIgnoreCase);
            }
            else
            {
                queryValues.Add($"{Uri.EscapeDataString(name)}={formattedValue}");
            }
        }

        return queryValues.Count == 0
            ? resolvedRoute
            : $"{resolvedRoute}?{string.Join("&", queryValues)}";
    }

    public static class AdminCarriers
    {
        public const string DeleteCarrierUrl = $"{Prefix}/admin/carriers/{{id}}";
    }

    public static class AdminDashboard
    {
        public const string GetAdminDashboardUrl = $"{Prefix}/admin/dashboard";
    }

    public static class AdminOperationManagers
    {
        public const string GetActiveOperationManagersUrl = $"{Prefix}/admin/operation-managers/active";
        public const string GetHandledCarriersUrl = $"{Prefix}/admin/operation-managers/{{operationManagerId}}/handled-carriers";
        public const string GetHandledShipmentsUrl = $"{Prefix}/admin/operation-managers/{{operationManagerId}}/handled-shipments";
        public const string GetOperationManagerByIdUrl = $"{Prefix}/admin/operation-managers/{{operationManagerId}}";
        public const string GetOperationManagersUrl = $"{Prefix}/admin/operation-managers";
    }

    public static class AdminUsers
    {
        public const string FilterUsersUrl = $"{Prefix}/admin/users";
        public const string GetUserDetailsUrl = $"{Prefix}/admin/users/{{userId}}";
    }

    public static class Authentication
    {
        public const string ChangePasswordUrl = $"{Prefix}/auth/change-password";
        public const string LoginUrl = $"{Prefix}/auth/login";
        public const string RefreshTokenUrl = $"{Prefix}/refresh-tokens";
        public const string RegisterUrl = $"{Prefix}/auth/register";
        public const string RevokeRefreshTokenUrl = $"{Prefix}/refresh-tokens/{{userId}}";
        public const string SignOutUrl = $"{Prefix}/auth/signout";
    }

    public static class Bundles
    {
        public const string CreateBundleUrl = $"{Prefix}/admin/bundles";
        public const string DeleteBundleUrl = $"{Prefix}/admin/bundles/{{bundleId}}";
        public const string GetBundleByIdUrl = $"{Prefix}/admin/bundles/{{bundleId}}";
        public const string GetBundlesUrl = $"{Prefix}/admin/bundles";
        public const string UpdateBundleUrl = $"{Prefix}/admin/bundles/{{bundleId}}";
    }

    public static class CarrierAnalytics
    {
        public const string GetCarrierRatingUrl = $"{Prefix}/carriers/{{carrierId}}/rating";
        public const string GetCarrierRevenueUrl = $"{Prefix}/carriers/{{carrierId}}/revenue";
    }

    public static class CarrierDashboard
    {
        public const string GetCarrierDashboardUrl = $"{Prefix}/carriers/{{carrierId}}/dashboard";
    }

    public static class CarrierProfile
    {
        public const string AddCarrierAdditionalInfoUrl = $"{Prefix}/carriers/additional-info";
        public const string GetCarrierProfileUrl = $"{Prefix}/carriers/{{carrierId}}/profile";
        public const string UpdateCarrierProfileUrl = $"{Prefix}/carriers/profile";
    }

    public static class CarrierShipments
    {
        public const string CompleteDeliveryUrl = $"{Prefix}/carriers/{{carrierId}}/shipments/{{shipmentId}}/complete-delivery";
        public const string CompletePickupUrl = $"{Prefix}/carriers/{{carrierId}}/shipments/{{shipmentId}}/complete-pickup";
        public const string GetCarrierShipmentByIdUrl = $"{Prefix}/carriers/{{carrierId}}/shipments/{{shipmentId}}";
        public const string GetCarrierShipmentsUrl = $"{Prefix}/carriers/{{carrierId}}/shipments";
        public const string UpdateCarrierStatusUrl = $"{Prefix}/carriers/{{carrierId}}/status";
    }

    public static class CarrierTrips
    {
        public const string GetCarrierTripByIdUrl = $"{Prefix}/carriers/{{carrierId}}/trips/{{tripId}}";
        public const string GetCarrierTripsUrl = $"{Prefix}/carriers/{{carrierId}}/trips";
    }

    public static class Cities
    {
        public const string CreateCityUrl = $"{Prefix}/admin/cities";
        public const string DeleteCityUrl = $"{Prefix}/admin/cities/{{cityId}}";
        public const string FilterCitiesUrl = $"{Prefix}/admin/cities";
        public const string GetCityByIdUrl = $"{Prefix}/admin/cities/{{cityId}}";
        public const string UpdateCityUrl = $"{Prefix}/admin/cities/{{cityId}}";
    }

    public static class Governments
    {
        public const string CreateGovernmentUrl = $"{Prefix}/admin/governments";
        public const string DeleteGovernmentUrl = $"{Prefix}/admin/governments/{{governmentId}}";
        public const string GetGovernmentByIdUrl = $"{Prefix}/admin/governments/{{governmentId}}";
        public const string GetGovernmentsUrl = $"{Prefix}/admin/governments";
        public const string UpdateGovernmentUrl = $"{Prefix}/admin/governments/{{governmentId}}";
    }

    public static class Locations
    {
        public const string GetCitiesByGovernmentUrl = $"{Prefix}/governments/{{governmentId}}/cities";
        public const string GetCountriesUrl = $"{Prefix}/countries";
        public const string GetCountryGovernmentsUrl = $"{Prefix}/countries/{{countryId}}/governments";
    }

    public static class OperationManagerCarriers
    {
        public const string AssignDeliveryCarrierUrl = $"{Prefix}/operation-managers/carriers/{{shipmentId}}/assign-delivery";
        public const string AssignPickupCarrierUrl = $"{Prefix}/operation-managers/carriers/{{shipmentId}}/assign-pickup";
        public const string FilterCarriersUrl = $"{Prefix}/operation-managers/carriers";
        public const string GetCarrierByIdUrl = $"{Prefix}/operation-managers/carriers/{{carrierId}}";
        public const string GetCarrierShipmentByIdUrl = $"{Prefix}/operation-managers/carriers/{{carrierId}}/shipments/{{shipmentId}}";
        public const string GetCarrierShipmentsUrl = $"{Prefix}/operation-managers/carriers/{{carrierId}}/shipments";
    }

    public static class OperationManagerDashboard
    {
        public const string GetOperationManagerDashboardUrl = $"{Prefix}/operation-managers/dashboard";
    }

    public static class OperationManagerProfile
    {
        public const string GetHandledCarriersUrl = $"{Prefix}/operation-managers/{{operationManagerId}}/handled-carriers";
        public const string GetHandledShipmentsUrl = $"{Prefix}/operation-managers/{{operationManagerId}}/handled-shipments";
        public const string GetOperationManagerByIdUrl = $"{Prefix}/operation-managers/{{operationManagerId}}";
    }

    public static class OperationManagerShipments
    {
        public const string ApproveShipmentUrl = $"{Prefix}/operation-managers/shipments/{{shipmentId}}/approve";
        public const string FilterShipmentsUrl = $"{Prefix}/operation-managers/shipments";
        public const string GetAssignedShipmentsUrl = $"{Prefix}/operation-managers/shipments/assigned";
        public const string GetShipmentByIdUrl = $"{Prefix}/operation-managers/shipments/{{shipmentId}}";
        public const string GetShipmentHistoriesUrl = $"{Prefix}/operation-managers/shipments/{{shipmentId}}/histories";
        public const string GetShipmentReviewQueueUrl = $"{Prefix}/operation-managers/shipments/review-queue";
        public const string RejectShipmentUrl = $"{Prefix}/operation-managers/shipments/{{shipmentId}}/reject";
        public const string ReviewShipmentUrl = $"{Prefix}/operation-managers/shipments/{{shipmentId}}/review";
    }

    public static class OperationManagerTrips
    {
        public const string GetCarrierTripByIdUrl = $"{Prefix}/carriers/{{carrierId}}/trips/{{tripId}}";
        public const string GetCarrierTripsUrl = $"{Prefix}/carriers/{{carrierId}}/trips";
        public const string StartDeliveryTripUrl = $"{Prefix}/operation-managers/trips/{{carrierId}}/start-delivery";
        public const string StartPickupTripUrl = $"{Prefix}/operation-managers/trips/{{carrierId}}/start-pickup";
    }

    public static class Roles
    {
        public const string CreateRoleUrl = $"{Prefix}/admin/roles";
        public const string DeleteRoleUrl = $"{Prefix}/admin/roles/{{roleId}}";
        public const string GetRoleByIdUrl = $"{Prefix}/admin/roles/{{roleId}}";
        public const string GetRoleMembersUrl = $"{Prefix}/admin/roles/{{roleId}}/members";
        public const string GetRolesUrl = $"{Prefix}/admin/roles";
        public const string UpdateRoleMembersUrl = $"{Prefix}/admin/roles/{{roleId}}/members";
        public const string UpdateRoleUrl = $"{Prefix}/admin/roles/{{roleId}}";
    }

    public static class Subscriptions
    {
        public const string GetBundleSubscribersUrl = $"{Prefix}/admin/subscriptions/bundles/{{bundleId}}/subscribers";
        public const string GetSubscriptionByIdUrl = $"{Prefix}/admin/subscriptions/{{subscriptionId}}";
    }

    public static class UserCarrierRatings
    {
        public const string RateDeliveryCarrierUrl = $"{Prefix}/shipments/{{shipmentId}}/rate-delivery-carrier";
        public const string RatePickupCarrierUrl = $"{Prefix}/shipments/{{shipmentId}}/rate-pickup-carrier";
    }

    public static class UserProfile
    {
        public const string GetUserDashboardUrl = $"{Prefix}/users/dashboard";
        public const string GetUserProfileUrl = $"{Prefix}/users/profile";
    }

    public static class UserShipments
    {
        public const string CancelShipmentUrl = $"{Prefix}/users/shipments/{{shipmentId}}";
        public const string CreateShipmentUrl = $"{Prefix}/users/shipments";
        public const string DeleteShipmentUrl = $"{Prefix}/users/shipments/{{shipmentId}}";
        public const string GetUserShipmentByIdUrl = $"{Prefix}/users/shipments/{{shipmentId}}";
        public const string IssueShipmentUrl = $"{Prefix}/users/shipments/{{shipmentId}}/issue";
        public const string TrackShipmentUrl = $"{Prefix}/users/shipments/{{trackingNumber}}";
        public const string UpdateShipmentUrl = $"{Prefix}/users/shipments/{{shipmentId}}";
    }

    public static class UserSubscriptions
    {
        public const string SubscribeToBundleUrl = $"{Prefix}/subscriptions/bundles/{{bundleId}}/subscription";
        public const string UnsubscribeFromBundleUrl = $"{Prefix}/subscriptions/bundles/{{bundleId}}/subscription";
    }

    public static class Vehicles
    {
        public const string CreateVehicleUrl = $"{Prefix}/admin/vehicles";
        public const string DeleteVehicleUrl = $"{Prefix}/admin/vehicles/{{vehicleId}}";
        public const string GetActiveVehiclesUrl = $"{Prefix}/admin/vehicles/active";
        public const string GetVehicleByIdUrl = $"{Prefix}/admin/vehicles/{{vehicleId}}";
        public const string GetVehicleByPlateNumberUrl = $"{Prefix}/admin/vehicles/plate-number/{{plateNumber}}";
        public const string GetVehiclesUrl = $"{Prefix}/admin/vehicles";
    }

    public static class Warehouses
    {
        public const string CreateWarehouseUrl = $"{Prefix}/admin/warehouses";
        public const string DeleteWarehouseUrl = $"{Prefix}/admin/warehouses/{{warehouseId}}";
        public const string GetWarehouseByIdUrl = $"{Prefix}/admin/warehouses/{{warehouseId}}";
        public const string GetWarehousesUrl = $"{Prefix}/admin/warehouses";
        public const string UpdateWarehouseUrl = $"{Prefix}/admin/warehouses/{{warehouseId}}";
    }
}

