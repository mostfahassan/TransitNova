
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Shipment;
using TransitNova.BusinessLayer.Interfaces.MarkerInterfaces;
using TransitNova.Domain.Contracts.Caching;
namespace TransitNova.BusinessLayer.Features.OperationManagerService.Queries.OperationManagerQueries
{
    public sealed record GetOperationManagerHandledShipmentsQuery(Guid OperationManagerId, int PageNumber, int PageSize) 
        : IQuery<Result<PagedResult<RetrieveShipmentSummaryDto>>>, ICachable
    {
        public string CacheKey => CacheKeys.OperationManagerHandledShipments(OperationManagerId, PageNumber, PageSize);
    }
   
}
