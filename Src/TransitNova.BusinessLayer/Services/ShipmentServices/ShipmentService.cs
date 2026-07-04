
using MediatR;
using TransitNova.BusinessLayer.DTOs.Shipment;
using TransitNova.BusinessLayer.Interfaces.Repositories.ShipmentRepository;
using TransitNova.BusinessLayer.Interfaces.Repositories.UserRepository;
using TransitNova.BusinessLayer.Interfaces.Services.ShipmentServices;
using TransitNova.BusinessLayer.Interfaces.Services.UnitOfWork;
using TransitNova.Domain.DomainExceptions;
using TransitNova.Domain.Entities.MainEntities;
namespace TransitNova.BusinessLayer.Services.ShipmentServices
{
    internal class ShipmentService(IShipmentCommandRepository shipmentCommand, IUnitOfWork unitOfWork, IReceiverRepository receiver,
     IUserQueryRepository user, IShipmentPricingServices pricingService) : IShipmentService
    {
        public async Task<Guid> PrepareShipmentCreationAsync(CreateShipmentDto Dto, Guid AppUserId, CancellationToken cancellationToken)
        {
            var senderId = await user.GetAppUserIdAsync(AppUserId, cancellationToken);

            //=== Preparation Creating Shipment ========//
            var receiverToCreate = ReceiverProfile.Create(Dto.Receiver.FirstName, Dto.Receiver.LastName, Dto.Receiver.Email, Dto.Receiver.PhoneNumber,
                 Dto.Receiver.Address, Dto.Receiver.CityId, senderId);

            //==== Calculate Shipment Cost and Estimated Delivery Date ========//
            var packageSpecification = Dto.PackageSpecification.ToDomain();
            var (cost, estimatedDeliveryDate) = pricingService.CalculateShipment(packageSpecification, Dto.ShipmentDeliveryType, Dto.TransportationMode);

            var shipment = Shipment.Create(senderId, receiverToCreate, packageSpecification, Dto.Currency, Dto.PickUpDate, Dto.DeliveryAddress, Dto.PickupAddress,
                 Dto.ShipmentDeliveryType, Dto.TransportationMode, Dto.PackageBundleId,Dto.PaymentId,Dto.PaymentMethod, cost, estimatedDeliveryDate);
            //==== Create Receiver =========//
            await receiver.CreateReceiverAsync(receiverToCreate, cancellationToken);

            //==== Create Shipment =========//
            await shipmentCommand.AddAsync(shipment, cancellationToken);

            //==== Save Changes and Commit =====//
            var createResult = await unitOfWork.SaveChangesAsync(cancellationToken);
            if (createResult <= 0) throw new DomainOperationException("Shipment Creation Failed", "SHIPMENT_CREATION_FAILED");

            return shipment.Id;
        }

        public void UpdateShipmentDetails(Shipment shipment, UpdateShipmentDto shipmentCommand)
        {
            bool needRecalculation = (shipment.Mode != shipmentCommand.TransportationMode && shipmentCommand.TransportationMode != null)
              || (shipment.ShipmentType != shipmentCommand.ShipmentType && shipmentCommand.ShipmentType != null)
              || (shipmentCommand.PackageSpecification != null && !shipmentCommand.PackageSpecification.Equals(shipment.PackageSpecification));

            //=== Recalculate Cost and Delivery Date if Necessary 

            (decimal? cost, DateTime? deliveryDate) = (null, null);
            if (needRecalculation)
            {
                var packageSpecification = shipmentCommand.PackageSpecification?.ToDomain() ?? shipment.PackageSpecification;

                var shipmentType = shipmentCommand.ShipmentType ?? shipment.ShipmentType;

                var transportationMode = shipmentCommand.TransportationMode ?? shipment.Mode;

                var result = pricingService.CalculateShipment(packageSpecification, shipmentType, transportationMode);

                cost = result.Item1;
                deliveryDate = result.Item2;
            }

            //=== Update Shipment ById 
            shipment.UpdateShipmentDetails(shipmentCommand.ReceiverId, shipmentCommand.DeliveryAddress, shipmentCommand.PickupAddress, shipmentCommand.TransportationMode,
                shipmentCommand.ShipmentType, shipmentCommand.PackageSpecification?.ToDomain(), cost, deliveryDate);

        }
    }
}
