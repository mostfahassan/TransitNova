using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.Interfaces.MarkerInterfaces;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Shipment;
using TransitNova.Domain.Contracts.Caching;

namespace TransitNova.BusinessLayer.Features.OperationManagerService.Queries.Shipments
{
    public sealed record GetAssignedShipmentsQuery(ShipmentFilterDto Dto) : IQuery<Result<PagedResult<RetrieveShipmentDto>>>, ICachable
    {
        public string CacheKey => CacheKeys.OperationManagers.AssignedShipments(Dto);
    }
}

