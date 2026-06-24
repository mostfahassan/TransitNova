using TransitNova.BusinessLayer.Common.CQRS;
using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Vehicle;
using TransitNova.BusinessLayer.Features.Vehicles.Queries;
using TransitNova.BusinessLayer.Interfaces.Repositories.VehicleRepository;
using TransitNova.Domain.Enums.Result;
namespace TransitNova.BusinessLayer.Features.Vehicles.Handlers.ApplyQueries
{
    public sealed class GetVehiclesByCarrierIdHandler(
        IVehicleQueryRepository vehicleRepository,
        ILogger<GetVehiclesByCarrierIdHandler> logger)
        : IQueryHandler<GetVehiclesByCarrierIdQuery, Result<List<VehicleDto>>>
    {
        public async Task<Result<List<VehicleDto>>> Handle(GetVehiclesByCarrierIdQuery request, CancellationToken ct)
        {
            if (request.CarrierId == Guid.Empty)
                return Result<List<VehicleDto>>.NotFound(Errors.CarrierNotFound("Carrier id is required."));

            var vehicles = await vehicleRepository.GetByCarrierIdAsync(request.CarrierId, ct);
            if (vehicles.Count == 0)
            {
                logger.LogInformation("No vehicles found for Carrier {UserId}", request.CarrierId);
                return Result<List<VehicleDto>>.Success([]);
            }

            return Result<List<VehicleDto>>.Success(vehicles);
        }
    }
}
