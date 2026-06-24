
using FluentValidation;
using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Carrier;
using TransitNova.BusinessLayer.Features.Carriers.Queries.Carrier;
using TransitNova.BusinessLayer.Interfaces.Repositories.CarrierRepository;
namespace TransitNova.BusinessLayer.Features.Carriers.Handlers.ApplyQueries
{
    public class FilterCarriersQueryHandler(
        ICarrierQueryRepository carrierRepo,
        ILogger<FilterCarriersQueryHandler> logger)
        : IQueryHandler<FilterCarriersQuery, Result<PagedResult<CarrierProfileDto>>>
    {
        public async Task<Result<PagedResult<CarrierProfileDto>>> Handle(
        FilterCarriersQuery request, CancellationToken ct)
        {
            logger.LogInformation("Filtering carriers with criteria: {@FilterCriteria}", request.FilterCriteria);
            var filteredCarriers = await carrierRepo.FilterByCriteriaAsync<CarrierProfileDto>(request.FilterCriteria ,tracked :false , ct);

            if (!filteredCarriers.Data.Any())
            {
                logger.LogInformation("No carriers found matching the filter criteria");
                var emptyResult = Result<PagedResult<CarrierProfileDto>>.Success(filteredCarriers);
                return emptyResult;

            }

            logger.LogInformation("Found {Count} carriers (Page {PageNumber}/{TotalPages})",
                filteredCarriers.Data.Count(),
                request.FilterCriteria.PageNumber,
                filteredCarriers.TotalPages);

            var result = Result<PagedResult<CarrierProfileDto>>.Success(filteredCarriers);
            return result;
        }
    }
   }

