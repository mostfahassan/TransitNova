using TransitNova.BusinessLayer.Common.Caching;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Shipment;
using TransitNova.BusinessLayer.Interfaces.MarkerInterfaces;

namespace TransitNova.BusinessLayer.Features.Carriers.Queries
{
    public sealed record GetCarrierShipmentDetailsQuery(Guid CarrierId, Guid ShipmentId)
        : IQuery<Result<RetrieveShipmentDto>>, ICachable
    {
        public string CacheKey => CacheKeys.CarrierShipmentDetails(CarrierId, ShipmentId);
    }
}
