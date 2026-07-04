using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.Interfaces.MarkerInterfaces;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.ShipmentStatusDto;
using TransitNova.Domain.Contracts.Caching;

namespace TransitNova.BusinessLayer.Features.OperationManagerService.Queries.Shipments
{
    public record GetShipmentHistoryQuery(Guid ShipmentId) : IQuery<Result<RetrieveShipmentStatusDto>>, ICachable
    {
        public string CacheKey => CacheKeys.OperationManagers.ShipmentHistories(ShipmentId);
    };
   
}
