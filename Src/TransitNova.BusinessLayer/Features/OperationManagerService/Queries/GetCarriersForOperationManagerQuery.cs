using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Carrier;
namespace TransitNova.BusinessLayer.Features.OperationManagerService.Queries
{
    public record GetCarriersForOperationManagerQuery(FilterCarrierDto FilterCriteria) : IQuery<Result<PagedResult<CarrierSummaryDetailsDto>>>;
}
