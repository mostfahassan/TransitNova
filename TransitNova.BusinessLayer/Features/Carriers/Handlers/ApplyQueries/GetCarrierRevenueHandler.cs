
using TransitNova.BusinessLayer.Common.CQRS;
using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.Interfaces.Repositories.CarrierRepository;
using TransitNova.BusinessLayer.Features.Carriers.Queries;
namespace TransitNova.BusinessLayer.Features.Carriers.Handlers.ApplyQueries
{
    public class GetCarrierRevenueHandler(ICarrierAnalyticsQueryRepository carrierRepo, ILogger<GetCarrierRatingQueryHandler> logger) 
        : IQueryHandler<GetCarrierRevenueQuery, Result<decimal>>
    {
        public async Task<Result<decimal>> Handle(GetCarrierRevenueQuery request, CancellationToken cancellationToken)
        {
            logger.LogDebug("Fetching rating for Carrier {UserId}", request.CarrierId);

            var revenue = await carrierRepo.GetCarrierRevenueAsync(request.CarrierId, cancellationToken);

            if (!revenue.HasValue)
            {
                logger.LogInformation("No ratings found for Carrier {UserId}", request.CarrierId);
                return Result<decimal>.Success(default);
            }

            logger.LogInformation("Carrier {UserId} rating: {Rating}", request.CarrierId, revenue.Value);
            return Result<decimal>.Success(revenue.Value);
        }
    }
}