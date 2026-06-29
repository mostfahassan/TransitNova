using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Shipment;
using TransitNova.BusinessLayer.Interfaces.MarkerInterfaces;
using TransitNova.Domain.Contracts.Caching;

namespace TransitNova.BusinessLayer.Features.UserOperations.Queries
{
    public record TrackShipmentQuery(string TrackingNumber) 
        : IQuery<Result<RetrieveShipmentSummaryDto>>, ICachable
    {
        public string CacheKey => CacheKeys.Shipments.ByTrackingNumber(TrackingNumber);
    }
   
}

