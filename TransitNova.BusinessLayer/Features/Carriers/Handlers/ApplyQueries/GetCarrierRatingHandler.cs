
using TransitNova.BusinessLayer.Common.CQRS;
using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.Interfaces.Repositories.CarrierRepository;
using TransitNova.BusinessLayer.Features.Carriers.Queries;
namespace TransitNova.BusinessLayer.Features.Carriers.Handlers.ApplyQueries
{
    public class GetCarrierRatingQueryHandler(
    ICarrierAnalyticsQueryRepository carrierRepo,
    ILogger<GetCarrierRatingQueryHandler> logger)
    : IQueryHandler<GetCarrierRatingQuery, Result<decimal>>
    {
        public async Task<Result<decimal>> Handle(GetCarrierRatingQuery request, CancellationToken ct)
        {
            logger.LogDebug("Fetching rating for Carrier {UserId}", request.CarrierId);

            var rating = await carrierRepo.GetAverageRatingAsync(request.CarrierId, ct);

            if (!rating.HasValue)
            {
                logger.LogInformation("No ratings found for Carrier {UserId}", request.CarrierId);
                return Result<decimal>.Success(default);
            }

            logger.LogInformation("Carrier {UserId} rating: {Rating}", request.CarrierId, rating.Value);
            return Result<decimal>.Success(rating.Value);
        }
    }
}