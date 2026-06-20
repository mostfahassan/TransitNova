using TransitNova.BusinessLayer.Common.CQRS;
using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Carrier;
using TransitNova.BusinessLayer.Features.Carriers.Queries;
using TransitNova.BusinessLayer.Interfaces.Repositories.CarrierRepository;
namespace TransitNova.BusinessLayer.Features.Carriers.Handlers.ApplyQueries
{
    public sealed class RetrieveCarriersInStatusQueryHandler(
        ICarrierQueryRepository carrierRepository ,
        ILogger<RetrieveCarriersInStatusQueryHandler> logger)
        : IQueryHandler<RetrieveCarriersByStatusQuery, Result<IEnumerable<CarrierProfileDto>>>
    {
        public async Task<Result<IEnumerable<CarrierProfileDto>>> Handle(RetrieveCarriersByStatusQuery request, CancellationToken cancellationToken)
        {
            if(!Enum.IsDefined(request.CarrierStatus))
            {
                return Result<IEnumerable<CarrierProfileDto>>.Forbidden(Errors.InvalidCarrierState("Invalid Status"));
            }

            var carrierInStatus = await carrierRepository.GetCarriersInStatusAsync(request.CarrierStatus, cancellationToken);
            if (!carrierInStatus.Any())
            {
                logger.LogInformation("No Carriers Found in {Status}" , request.CarrierStatus);
                return Result<IEnumerable<CarrierProfileDto>>.Success([]);
            }
            logger.LogInformation("{Count} Carriers Found in {Status}", carrierInStatus.Count(),request.CarrierStatus);
            return Result<IEnumerable<CarrierProfileDto>>.Success(carrierInStatus);
        }
    }
}
