using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Shipment;
using TransitNova.BusinessLayer.Common.Caching;
using TransitNova.BusinessLayer.Interfaces.MarkerInterfaces;

namespace TransitNova.BusinessLayer.Features.UserOperations.Queries
{
    public record GetUserShipmentQuery(Guid AppUserId , Guid ShipmentId) : IQuery<Result<RetrieveShipmentDto>>, ICachable
    {
        public string CacheKey => CacheKeys.UserShipment(AppUserId, ShipmentId);
    }


}
