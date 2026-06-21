
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Carrier;
using TransitNova.BusinessLayer.Interfaces.MarkerInterfaces;
using TransitNova.Domain.Contracts.Caching;

namespace TransitNova.BusinessLayer.Features.OperationManagerService.Queries.OperationManagerQueries
{
    public sealed record GetOperationManagerHandledCarriersQuery(Guid OperationManagerId , int PageNumber , int PageSize) 
        : IQuery<Result<PagedResult<CarrierSummaryDetailsDto>>>, ICachable
    {
        public string CacheKey => CacheKeys.OperationManagerHandledCarriers(OperationManagerId, PageNumber, PageSize);
    }
   
}
