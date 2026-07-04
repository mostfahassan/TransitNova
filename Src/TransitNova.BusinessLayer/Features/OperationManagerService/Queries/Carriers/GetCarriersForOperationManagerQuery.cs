using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.Interfaces.MarkerInterfaces;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Carrier;
using TransitNova.Domain.Contracts.Caching;
namespace TransitNova.BusinessLayer.Features.OperationManagerService.Queries.Carriers
{
    public record GetCarriersForOperationManagerQuery(FilterCarrierDto FilterCriteria) : IQuery<Result<PagedResult<CarrierSummaryDetailsDto>>>, ICachable
    {
        public string CacheKey => CacheKeys.OperationManagers.FilterCarriers(FilterCriteria);
    }
}
