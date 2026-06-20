
using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Carrier;
using TransitNova.BusinessLayer.Features.OperationManagerService.Commands;
using TransitNova.BusinessLayer.Interfaces.Repositories.CarrierRepository;
namespace TransitNova.BusinessLayer.Features.OperationManagerService.Handlers.Commands
{
    public class FilterCarriersForOperationManagerCommandHandler(
     ICarrierQueryRepository carrierRepo,
     ILogger<FilterCarriersForOperationManagerCommandHandler> logger)

     : ICommandHandler<GetCarriersForOperationManagerCommand, Result<PagedResult<CarrierSummaryDetailsDto>>>
    {
        public async Task<Result<PagedResult<CarrierSummaryDetailsDto>>> Handle(
        GetCarriersForOperationManagerCommand request, CancellationToken ct)
        {
            logger.LogInformation("Filtering carriers with criteria: {@FilterCriteria}", request.FilterCriteria);
            var carrierFiltration = await carrierRepo.FilterByCriteriaAsync<CarrierSummaryDetailsDto>(request.FilterCriteria, tracked: false, ct);
            if (carrierFiltration.Data.Count() == 0  )
            {
                logger.LogInformation("No carriers found matching the filter criteria");
                return Result<PagedResult<CarrierSummaryDetailsDto>>.Success(carrierFiltration);

            }
            logger.LogInformation("Found {Count} carriers (Page {PageNumber}/{TotalPages})",
                carrierFiltration.Data.Count(),
                request.FilterCriteria.PageNumber,
                carrierFiltration.TotalPages);

            return Result<PagedResult<CarrierSummaryDetailsDto>>.Success(carrierFiltration);
        }
    }
}
