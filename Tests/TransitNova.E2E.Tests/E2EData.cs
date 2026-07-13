namespace TransitNova.E2E.Tests;

internal static class E2EData
{
    public static object CreateShipmentBody(int cityId, decimal weight = 2.5m)
        => new
        {
            receiver = new
            {
                firstName = "Automation",
                senderId = Guid.Empty,
                lastName = "Receiver",
                email = "shipment.receiver@transitnova.test",
                phoneNumber = "+201009876543",
                address = new
                {
                    mainAddress = "Alexandria delivery district",
                    secondaryAddress = "Building 8",
                    street = "Delivery Street"
                },
                cityId
            },
            packageSpecification = new
            {
                weight,
                width = 20m,
                height = 15m,
                length = 30m
            },
            currency = "EGP",
            pickUpDate = DateTime.UtcNow.AddDays(2),
            transportationMode = "Land",
            shipmentDeliveryType = "Standard",
            deliveryAddress = new
            {
                mainAddress = "Alexandria delivery district",
                secondaryAddress = "Building 8",
                street = "Delivery Street"
            },
            pickupAddress = new
            {
                mainAddress = "Cairo pickup district",
                secondaryAddress = "Warehouse 3",
                street = "Pickup Street"
            },
            paymentId = Guid.Empty,
            paymentMethod = "CreditCard"
        };
}

