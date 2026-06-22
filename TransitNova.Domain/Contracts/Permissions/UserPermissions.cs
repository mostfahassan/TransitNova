namespace TransitNova.Domain.Contracts.Permissions
{
    public static class UserPermissions
    {
        public const string UserCanAddShipment = "User.AddShipment";
        public const string UserCanShowHisDashboard = "User.ShowHisDashboard";
        public const string ShipmentOwner = nameof(ShipmentOwner);
        public const string UserCanCancelShipment = "User.CancelShipment";
        public const string UserCanUpdateShipment = "User.UpdateShipment";
        public const string UserCanDeleteShipment = "User.DeleteShipment";
        public const string UserCanTrackShipment = "User.TrackShipment";
        public const string UserCanViewShipmentDetails = "User.ViewShipmentDetails";
        public const string UserCanIssueShipment = "User.IssueShipment";
        public const string UserCanUpdateProfile = "User.UpdateProfile";
        public const string UserCanViewProfile = "User.ViewProfile";
        public const string UserCanSubscribeBundle = "User.SubscribeBundle";
        public const string UserCanUnSubscribeBundle = "User.UnSubscribeBundle";
        public const string UserCanRateCarrier = "User.ReviewCarrier";
        public const string CanRatePickupCarrier = "User.RatePickupCarrier";
        public const string CanRateDeliveryCarrier = "User.RateDeliveryCarrier";
     

        public static IReadOnlyList<string> All =>
                [
                    UserCanShowHisDashboard,
                    UserCanAddShipment,
                    UserCanCancelShipment,
                    UserCanUpdateShipment,
                    UserCanDeleteShipment,
                    UserCanTrackShipment,
                    UserCanViewShipmentDetails,
                    UserCanIssueShipment,
                    UserCanUpdateProfile,
                    UserCanViewProfile,
                    UserCanSubscribeBundle,
                    UserCanUnSubscribeBundle,
                    UserCanRateCarrier,
                    CanRatePickupCarrier,
                    CanRateDeliveryCarrier, 
                ];
    }
}
