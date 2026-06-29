using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.ShipmentStatusDto;
using TransitNova.BusinessLayer.Interfaces.MarkerInterfaces;
using TransitNova.Domain.Contracts.Caching;

namespace TransitNova.BusinessLayer.Features.OperationManagerService.Queries.Shipments
{
    public record GetShipmentHistoriesQuery(Guid ShipmentId) : IQuery<Result<IEnumerable<RetrieveShipmentStatusDto>>>, ICachable
    {
        public string CacheKey => CacheKeys.OperationManagers.ShipmentHistories(ShipmentId);
    }
   
}

