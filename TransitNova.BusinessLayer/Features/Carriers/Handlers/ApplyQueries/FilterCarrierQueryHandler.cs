
using FluentValidation;
using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Carrier;
using TransitNova.BusinessLayer.Features.Carriers.Queries;
using TransitNova.BusinessLayer.Interfaces.Repositories.CarrierRepository;
namespace TransitNova.BusinessLayer.Features.Carriers.Handlers.ApplyQueries
{
    public class FilterCarriersQueryCommand(
        ICarrierQueryRepository carrierRepo,
        ILogger<FilterCarriersQueryCommand> logger)
        : IQueryHandler<FilterCarriersQuery, Result<PagedResult<CarrierProfileDto>>>
    {
        public async Task<Result<PagedResult<CarrierProfileDto>>> Handle(
        FilterCarriersQuery request, CancellationToken ct)
        {
            logger.LogInformation("Filtering carriers with criteria: {@FilterCriteria}", request.FilterCriteria);
            var carrierFiltration = await carrierRepo.FilterByCriteriaAsync<CarrierProfileDto>(request.FilterCriteria ,tracked :false , ct);

            if (!carrierFiltration.Data.Any())
            {
                logger.LogInformation("No carriers found matching the filter criteria");
                var emptyResult = Result<PagedResult<CarrierProfileDto>>.Success(carrierFiltration);
                return emptyResult;

            }

            logger.LogInformation("Found {Count} carriers (Page {PageNumber}/{TotalPages})",
                carrierFiltration.Data.Count(),
                request.FilterCriteria.PageNumber,
                carrierFiltration.TotalPages);

            var result = Result<PagedResult<CarrierProfileDto>>.Success(carrierFiltration);
            return result;
        }
    }
   }

