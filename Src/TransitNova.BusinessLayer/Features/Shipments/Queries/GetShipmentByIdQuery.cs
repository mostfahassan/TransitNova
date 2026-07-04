using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.Interfaces.MarkerInterfaces;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Shipment;
using TransitNova.Domain.Contracts.Caching;

namespace TransitNova.BusinessLayer.Features.Shipments.Queries
{
    public record GetShipmentByIdQuery(Guid ShipmentId) :
        IQuery<Result<RetrieveShipmentDto>>, ICachable
    {
        public string CacheKey => CacheKeys.Shipments.ById(ShipmentId);
    }
}
