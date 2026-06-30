
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.Interfaces.MarkerInterfaces;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Shipment;
using TransitNova.Domain.Contracts.Caching;
namespace TransitNova.BusinessLayer.Features.OperationManagerService.Queries.OperationManagerQueries
{
    public sealed record GetOperationManagerHandledShipmentsQuery(Guid OperationManagerId, int PageNumber, int PageSize) 
        : IQuery<Result<PagedResult<RetrieveShipmentSummaryDto>>>, ICachable
    {
        public string CacheKey => CacheKeys.OperationManagers.HandledShipments(OperationManagerId, PageNumber, PageSize);
    }
   
}

