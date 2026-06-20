using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.ShipmentStatusDto;
using TransitNova.BusinessLayer.Common.Caching;
using TransitNova.BusinessLayer.Interfaces.MarkerInterfaces;

namespace TransitNova.BusinessLayer.Features.OperationManagerService.Queries
{
    public record GetShipmentHistoriesQuery(Guid ShipmentId) : IQuery<Result<IEnumerable<RetrieveShipmentStatusDto>>>, ICachable
    {
        public string CacheKey => CacheKeys.OperationManagerShipmentHistories(ShipmentId);
    }
   
}
