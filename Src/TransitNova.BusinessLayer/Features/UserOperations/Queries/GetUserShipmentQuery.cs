using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Shipment;
using TransitNova.BusinessLayer.Interfaces.MarkerInterfaces;
using TransitNova.Domain.Contracts.Caching;

namespace TransitNova.BusinessLayer.Features.UserOperations.Queries
{
    public record GetUserShipmentQuery(Guid AppUserId , Guid ShipmentId) : IQuery<Result<RetrieveShipmentDto>>, ICachable
    {
        public string CacheKey => CacheKeys.Users.Shipment(AppUserId, ShipmentId);
    }


}

