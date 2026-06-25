
using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Carrier;
using TransitNova.BusinessLayer.Features.OperationManagerService.Queries.Carriers;
using TransitNova.BusinessLayer.Interfaces.Repositories.CarrierRepository;
namespace TransitNova.BusinessLayer.Features.OperationManagerService.Handlers.Queries
{
    public class FilterCarriersForOperationManagerCommandHandler(
     ICarrierQueryRepository carrierRepo,
     ILogger<FilterCarriersForOperationManagerCommandHandler> logger)

     : IQueryHandler<GetCarriersForOperationManagerQuery, Result<PagedResult<CarrierSummaryDetailsDto>>>
    {
        public async Task<Result<PagedResult<CarrierSummaryDetailsDto>>> Handle(
        GetCarriersForOperationManagerQuery request, CancellationToken ct)
        {
            logger.LogInformation("Filtering carriers with criteria: {@FilterCriteria}", request.FilterCriteria);
            var filteredCarriers = await carrierRepo.FilterByCriteriaAsync<CarrierSummaryDetailsDto>(request.FilterCriteria, tracked: false, ct);
            if (!filteredCarriers.Data.Any())
            {
                logger.LogInformation("No carriers found matching the filter criteria");
                return Result<PagedResult<CarrierSummaryDetailsDto>>.Success(filteredCarriers);

            }
            logger.LogInformation("Found {Count} carriers (Page {PageNumber}/{TotalPages})",
                filteredCarriers.Data.Count(),
                request.FilterCriteria.PageNumber,
                filteredCarriers.TotalPages);

            return Result<PagedResult<CarrierSummaryDetailsDto>>.Success(filteredCarriers);
        }
    }
}
