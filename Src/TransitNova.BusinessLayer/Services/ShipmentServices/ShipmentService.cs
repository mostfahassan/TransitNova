using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Payment;
using TransitNova.BusinessLayer.DTOs.Shipment;
using TransitNova.BusinessLayer.Interfaces.Repositories.PaymentInvoiceRepository;
using TransitNova.BusinessLayer.Interfaces.Repositories.ShipmentRepository;
using TransitNova.BusinessLayer.Interfaces.Repositories.UserRepository;
using TransitNova.BusinessLayer.Interfaces.Services.BundleService;
using TransitNova.BusinessLayer.Interfaces.Services.PaymentService;
using TransitNova.BusinessLayer.Interfaces.Services.ShipmentServices;
using TransitNova.Domain.Entities.MainEntities;
using TransitNova.Domain.Enums.Payment;
namespace TransitNova.BusinessLayer.Services.ShipmentServices
{
    internal class ShipmentService(IShipmentCommandRepository shipmentCommand,
        IPaymentRepositoryCommand payment,
        IReceiverRepository receiver,
        IPaymentService paymentService,
        IUserQueryRepository user,
        IShipmentPricingServices pricingService,
        IBundleBenefitService bundleBenefitService) : IShipmentService
    {
        public async Task<(Result<InvoiceDto>, string)> HandleShipmentCreation(CreateShipmentDto Dto, Guid AppUserId, CancellationToken cancellationToken)
        {

            var senderId = await user.GetAppUserIdAsync(AppUserId, cancellationToken);

            //=== Preparation Creating Shipment
            var receiverToCreate = ReceiverProfile.Create(Dto.Receiver.FirstName, Dto.Receiver.LastName, Dto.Receiver.Email, Dto.Receiver.PhoneNumber,
                 Dto.Receiver.Address.ToDomain(), Dto.Receiver.CityId, senderId);

            //==== Calculate Shipment Cost and Estimated Delivery Date
            var packageSpecification = Dto.PackageSpecification.ToDomain();
            var (cost, estimatedDeliveryDate) = pricingService.CalculateShipment(packageSpecification, Dto.ShipmentDeliveryType, Dto.TransportationMode);
            var benefit = await bundleBenefitService.CalculateShipmentBenefitAsync(senderId, cost, Dto.PackageSpecification, cancellationToken);
            var payableCost = benefit.FinalShippingCost;

            //=== Initialize Shipment Creation
            var shipment = Shipment.Create(senderId, receiverToCreate, packageSpecification, Dto.Currency, Dto.PickUpDate, Dto.DeliveryAddress.ToDomain(), Dto.PickupAddress.ToDomain(),
                 Dto.ShipmentDeliveryType, Dto.TransportationMode, Dto.PaymentMethod);

            //=== Initialize Payment Command
            var paymentRequest = new CreatePaymentDto
            {
              ReferenceId = shipment.Id,
              PaymentMethod = Dto.PaymentMethod,
              Cost = payableCost,
              Currency = Dto.Currency
            };


            var response = await paymentService.Pay(paymentRequest, cancellationToken);

            if (response is null)
                return (Result<InvoiceDto>.Failure(Errors.FailedOperation("Payment operation failed")), string.Empty);

            if (response.IsFailure || response.Data is null)
                return (Result<InvoiceDto>.Failure(response.Error ?? Errors.FailedOperation(response.Message ?? "Payment operation failed")), string.Empty);

            if (!TryParsePaymentValues(response.Data, out var paymentMethod, out var paymentStatus))
            {
                return (Result<InvoiceDto>.Failure(Errors.FailedOperation("Payment response contains invalid payment values.")), string.Empty);
            }

            //=== Update Shipment with Payment Details
            shipment.SetShipmentCost(response.Data.Amount, estimatedDeliveryDate);
            shipment.SetPaymentId(response.Data.PaymentId);

            var paymentInvoice = PaymentInvoice.Create(
                response.Data.PaymentId,
                response.Data.ReferenceId,
                senderId,
                response.Data.Amount,
                response.Data.Commission,
                response.Data.TotalAmount,
                paymentMethod,
                paymentStatus,
                response.Data.ReferenceType!,
                response.Data.PaidAt,
                response.Data.Notes,
                benefit.BundleSubscriptionId,
                benefit.BundleId,
                benefit.BundleName,
                benefit.OriginalShippingCost,
                benefit.DiscountPercentage,
                benefit.DiscountAmount,
                response.Data.Amount,
                benefit.SubscriptionBenefitApplied);

            //==== Create Receiver
            await receiver.CreateReceiverAsync(receiverToCreate, cancellationToken);

            //==== Create Shipment
            await shipmentCommand.AddAsync(shipment, cancellationToken);
            await payment.CreateInvoice(paymentInvoice, cancellationToken);

            var invoice = new InvoiceDto
            {
                PaymentId = response.Data.PaymentId,
                ReferenceId = response.Data.ReferenceId,
                ReferenceType = response.Data.ReferenceType,
                Amount = response.Data.Amount,
                Commission = response.Data.Commission,
                TotalAmount = response.Data.TotalAmount,
                PaymentMethod = response.Data.PaymentMethod,
                Status = response.Data.Status,
                PaidAt = response.Data.PaidAt,
                Notes = response.Data.Notes,
                BundleSubscriptionId = benefit.BundleSubscriptionId,
                BundleId = benefit.BundleId,
                BundleName = benefit.BundleName,
                OriginalShippingCost = benefit.OriginalShippingCost,
                DiscountPercentage = benefit.DiscountPercentage,
                DiscountAmount = benefit.DiscountAmount,
                FinalShippingCost = response.Data.Amount,
                SubscriptionBenefitApplied = benefit.SubscriptionBenefitApplied,
                SubscriptionBenefitMessage = benefit.SubscriptionBenefitMessage
            };

            return (Result<InvoiceDto>.Success(invoice), shipment.TrackingNumber);
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

            shipment.UpdateShipmentDetails(shipmentCommand.ReceiverId, shipmentCommand.DeliveryAddress?.ToDomain(), shipmentCommand.PickupAddress?.ToDomain(), shipmentCommand.TransportationMode,
                shipmentCommand.ShipmentType, shipmentCommand.PackageSpecification?.ToDomain(), cost, deliveryDate);

        }

        private static bool TryParsePaymentValues(InvoiceDto invoice, out PaymentMethod paymentMethod, out PaymentStatus paymentStatus)
        {
            var hasPaymentMethod = Enum.TryParse(invoice.PaymentMethod, ignoreCase: true, out paymentMethod);
            var hasPaymentStatus = Enum.TryParse(invoice.Status, ignoreCase: true, out paymentStatus);
            return hasPaymentMethod && hasPaymentStatus;
        }
    }
}
