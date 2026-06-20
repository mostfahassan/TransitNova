using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Shipment;

using TransitNova.BusinessLayer.Common.Caching;
using TransitNova.BusinessLayer.Interfaces.MarkerInterfaces;

namespace TransitNova.BusinessLayer.Features.UserOperations.Queries
{
    public record TrackShipmentQuery(string TrackingNumber) 
        : IQuery<Result<RetrieveShipmentSummaryDto>>, ICachable
    {
        public string CacheKey => CacheKeys.ShipmentByTrackingNumber(TrackingNumber);
    }
   
}
