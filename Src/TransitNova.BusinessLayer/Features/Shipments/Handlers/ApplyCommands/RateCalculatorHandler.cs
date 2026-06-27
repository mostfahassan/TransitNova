using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.Features.Shipments.Commands;
using TransitNova.BusinessLayer.Interfaces.Services.ShipmentServices;

namespace TransitNova.BusinessLayer.Features.Shipments.Handlers.ApplyCommands
{
    public sealed class RateCalculatorHandler(
        IShipmentPricingServices pricingService,
        ILogger<RateCalculatorHandler> logger) : ICommandHandler<RateCalculatorCommand, Result<decimal>>
    {
        public Task<Result<decimal>> Handle(RateCalculatorCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation(
                "Calculating shipment rate. TransportationMode: {TransportationMode}, ShipmentDeliveryType: {ShipmentDeliveryType}, Weight: {Weight}, Width: {Width}, Height: {Height}, Length: {Length}",
                request.Dto.TransportationMode,
                request.Dto.ShipmentDeliveryType,
                request.Dto.PackageSpecification.Weight,
                request.Dto.PackageSpecification.Width,
                request.Dto.PackageSpecification.Height,
                request.Dto.PackageSpecification.Length);

            var (cost, estimatedDeliveryDate) = pricingService.CalculateShipment(
                request.Dto.PackageSpecification.ToDomain(),
                request.Dto.ShipmentDeliveryType,
                request.Dto.TransportationMode);

            logger.LogInformation(
                "Shipment rate calculated. Cost: {Cost}, EstimatedDeliveryDate: {EstimatedDeliveryDate}",
                cost,
                estimatedDeliveryDate);

            return Task.FromResult(Result<decimal>.Success(cost));
        }
    }
}
