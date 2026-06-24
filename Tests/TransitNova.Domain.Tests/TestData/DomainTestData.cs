using TransitNova.Domain.Entities.Common;
using TransitNova.Domain.Entities.MainEntities;
using TransitNova.Domain.Enums.Shipment;

namespace TransitNova.Domain.Tests.TestData;

internal static class DomainTestData
{
    internal static ReceiverProfile CreateReceiver(Guid? senderId = null) =>
        ReceiverProfile.Create(
            "Mona",
            "Ali",
            "mona@example.com",
            "+201000000001",
            "Cairo",
            1,
            senderId ?? Guid.NewGuid());

    internal static Shipment CreateShipment(Guid? senderId = null) =>
        Shipment.Create(
            senderId ?? Guid.NewGuid(),
            CreateReceiver(senderId),
            new PackageSpecification(10m, 2m, 3m, 4m),
            Currency.EGP,
            DateTime.UtcNow.AddDays(1),
            "Delivery address",
            "Pickup address",
            enShipmentType.Standard,
            TransportationMode.Land,
            null,
            250m,
            DateTime.UtcNow.AddDays(3));

    internal static Shipment CreatePickupAssignedShipment()
    {
        var shipment = CreateShipment();
        shipment.ApproveShipment(Guid.NewGuid());
        shipment.AssignToCarrier(ShipmentStatuses.AssignedToPickUpCarrier, Guid.NewGuid(), Guid.NewGuid());
        return shipment;
    }

    internal static Shipment CreateInWarehouseShipment()
    {
        var shipment = CreatePickupAssignedShipment();
        shipment.AssignedAsPickupTrip(Guid.NewGuid(), Guid.NewGuid());
        shipment.DeliveredToWarehouse(Guid.NewGuid());
        return shipment;
    }

    internal static Shipment CreateDeliveredShipment()
    {
        var shipment = CreateInWarehouseShipment();
        shipment.AssignToCarrier(ShipmentStatuses.AssignedToDeliveryCarrier, Guid.NewGuid(), Guid.NewGuid());
        shipment.AssignedAsDeliveryTrip(Guid.NewGuid(), Guid.NewGuid());
        shipment.Delivered(Guid.NewGuid());
        return shipment;
    }
}
