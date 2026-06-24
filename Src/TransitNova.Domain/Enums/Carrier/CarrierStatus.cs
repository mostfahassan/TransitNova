namespace TransitNova.Domain.Enums.Carrier
{
    public enum CarrierStatus
    {
        Available = 1,
        Unavailable = 2,
        AssignedToPickUpShipment = 3,
        AssignedToDeliveryShipment = 4,
        InActive = 5,
        OnTrip = 6,
    }
}
