using TransitNova.Domain.Entities.Common;
using TransitNova.Domain.Entities.MainEntities;
using TransitNova.Domain.Enums.Payment;
using TransitNova.Domain.Enums.Shipment;

namespace TransitNova.ApplicationLayer.Tests.TestData;

internal static class ShipmentTestData
{
    internal static Shipment CreateShipment()
    {
        var senderId = Guid.NewGuid();
        var receiver = ReceiverProfile.Create(
            "Mona",
            "Ali",
            "mona@example.com",
            "01000000000",
            "Cairo",
            1,
            senderId);

        return Shipment.Create(
            senderId,
            receiver,
            new PackageSpecification(10, 10, 10, 5),
            Currency.EGP,
            DateTime.UtcNow.AddDays(1),
            "Delivery Address",
            "Pickup Address",
            enShipmentType.Standard,
            TransportationMode.Land,
            null,
            Guid.NewGuid(),
            PaymentMethod.MobileWallets);
    }

    internal static Shipment CreateDeliveredShipment()
    {
        var shipment = CreateShipment();
        var managerId = Guid.NewGuid();
        var carrierId = Guid.NewGuid();
        shipment.ApproveShipment(managerId);
        shipment.AssignToCarrier(ShipmentStatuses.AssignedToDeliveryCarrier, carrierId, managerId);
        shipment.AssignedAsDeliveryTrip(Guid.NewGuid(), carrierId);
        shipment.Delivered(carrierId);
        return shipment;
    }
}
