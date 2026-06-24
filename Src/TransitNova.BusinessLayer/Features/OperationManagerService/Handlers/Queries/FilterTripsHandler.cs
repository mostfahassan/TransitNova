using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Trips;
using TransitNova.BusinessLayer.Features.OperationManagerService.Queries;
using TransitNova.BusinessLayer.Interfaces.Repositories.TripRepository;
namespace TransitNova.BusinessLayer.Features.OperationManagerService.Handlers.Queries
{
    public sealed class FilterTripsHandler(ITripQueryRepository tripRepository,ILogger<FilterTripsHandler> logger)
        : IQueryHandler<FilterTripsQuery, Result<PagedResult<TripDetailsDto>>>
    {
        public async Task<Result<PagedResult<TripDetailsDto>>> Handle(FilterTripsQuery request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Filtering trips. PageNumber: {PageNumber}, PageSize: {PageSize}", request.Filter.PageNumber, request.Filter.PageSize);

            var trips = await tripRepository.FilterTripsAsync(request.Filter, cancellationToken);

            logger.LogInformation("Trips filtered successfully. TotalCount: {TotalCount}", trips.TotalCount);

            return Result<PagedResult<TripDetailsDto>>.Success(trips);
        }
    }
}
