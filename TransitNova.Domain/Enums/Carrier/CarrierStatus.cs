namespace TransitNova.Domain.Enums.Carrier
{
    public enum CarrierStatus
    {
        Available = 1,
        Unavailable = 2,
        AssignedToPickUpShipment = 3,
        AssignedToDeliveryShipment = 4,
        OnLeave = 4,
        InActive = 5,
        Vacation = 6,
        OnTrip = 7,
    }
}
