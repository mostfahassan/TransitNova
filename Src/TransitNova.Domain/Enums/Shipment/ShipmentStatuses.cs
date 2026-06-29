namespace TransitNova.Domain.Enums.Shipment
{
    public enum ShipmentStatuses
    {
        Pending = 0,            
        Approved = 1, 
        Rejected = 2,
        InWarehouse = 3,        
        OnHold = 4,             
        AssignedToPickUpCarrier = 5,
        AssignedToDeliveryCarrier = 6,
        InTransit = 7,         
        OutForDelivery = 8,     
        OutForPickup = 9,     
        Delivered = 10, 
        FailedDelivery = 11,  
        Issue = 12,
        Cancelled = 13,
        Deleted = 14,
        PickedUp = 15,

    }
}