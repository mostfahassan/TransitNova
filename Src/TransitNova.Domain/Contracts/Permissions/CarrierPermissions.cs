namespace TransitNova.Domain.Contracts.Permissions
{
    public static class CarrierPermissions
    {
        public const string CanViewDashboard = "Carrier.CanViewDashboard";
        public const string ViewProfile = "Carrier.ViewProfile";
        public const string UpdateProfile = "Carrier.UpdateProfile";
        public const string ViewAssignedShipments = "Carrier.ViewAssignedShipments";
        public const string ViewShipmentSummary = "Carrier.ViewShipmentSummary";
        public const string ViewShipmentDetails = "Carrier.ViewShipmentDetails";
        public const string MarkShipmentPickedUp = "Carrier.MarkShipmentPickedUp";
        public const string MarkShipmentDelivered = "Carrier.MarkShipmentDelivered";
        public const string ViewTrips = "Carrier.ViewTrips";
        public const string ViewTripDetails = "Carrier.ViewTripDetails";
        public const string ViewAssignedTrips = "Carrier.ViewAssignedTrips";
        public const string StartPickupTrip = "Carrier.StartPickupTrip";
        public const string StartDeliveryTrip = "Carrier.StartDeliveryTrip";
        public const string CanCompletePickupShipment = "Carrier.CanCompletePickupShipment";
        public const string CanCompleteDeliveryShipment = "Carrier.CanCompleteDeliveryShipment";
        public const string ViewStatistics = "Carrier.ViewStatistics";
        public const string ViewReviews = "Carrier.ViewHisReviews";
        public const string ViewRating = "Carrier.ViewRating";
        public const string CanViewRevenue = "Carrier.CanViewRevenue";
        public const string IsCarrierOwner = "Carrier.IsCarrierOwner";
        public const string HasCompletedProfile = "Carrier.HasCompletedProfile";
        public const string CanUpdateStatus = "Carrier.CanUpdateStatus";
        public const string CanAddAdditionalInfo = "Carrier.CanAddAdditionalInfo";



        public static readonly string[] All =
        [
                CanViewDashboard,
                ViewProfile,
                UpdateProfile,
                ViewAssignedShipments,
                ViewShipmentSummary,
                ViewShipmentDetails,
                MarkShipmentPickedUp,
                MarkShipmentDelivered,
                ViewTrips,
                ViewTripDetails,
                ViewAssignedTrips,
                StartPickupTrip,
                StartDeliveryTrip,
                CanCompletePickupShipment,
                CanCompleteDeliveryShipment,
                ViewStatistics,
                ViewReviews,
                CanViewRevenue,
                IsCarrierOwner,
                CanUpdateStatus,
                CanAddAdditionalInfo,
                ViewRating

        ];
    }
}
