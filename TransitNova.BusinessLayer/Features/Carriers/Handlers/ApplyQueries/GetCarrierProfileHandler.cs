using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Carrier;
using TransitNova.BusinessLayer.Features.Carriers.Queries;
using TransitNova.BusinessLayer.Interfaces.Repositories.CarrierRepository;
namespace TransitNova.BusinessLayer.Features.Carriers.Handlers.ApplyQueries
{
    public sealed class GetCarrierProfileHandler(
        ICarrierQueryRepository carrierRepository,
        ILogger<GetCarrierProfileHandler> logger)
        : IQueryHandler<GetCarrierProfileQuery, Result<CarrierProfileDto>>
    {
        public async Task<Result<CarrierProfileDto>> Handle(GetCarrierProfileQuery request, CancellationToken ct)
        {
            var carrier = await carrierRepository.GetCarrierDetailsAsync<CarrierProfileDto>(request.CarrierId, ct);
            if (carrier is null)
            {
                logger.LogWarning("Carrier profile not found for AppUser {CarrierId}", request.CarrierId);
                var notFoundResult = Result<CarrierProfileDto>.NotFound(Errors.CarrierNotFound("Carrier profile not found."));
                return notFoundResult;
            }

            var result = Result<CarrierProfileDto>.Success(carrier);
            return result;
        }
    }
}
