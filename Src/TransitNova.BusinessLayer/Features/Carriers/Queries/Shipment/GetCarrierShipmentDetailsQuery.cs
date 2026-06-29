using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Shipment;
using TransitNova.BusinessLayer.Interfaces.MarkerInterfaces;
using TransitNova.Domain.Contracts.Caching;

namespace TransitNova.BusinessLayer.Features.Carriers.Queries.Shipment
{
    public sealed record GetCarrierShipmentDetailsQuery(Guid CarrierId, Guid ShipmentId)
        : IQuery<Result<RetrieveShipmentDto>>, ICachable
    {
        public string CacheKey => CacheKeys.Carriers.ShipmentDetails(CarrierId, ShipmentId);
    }
}

