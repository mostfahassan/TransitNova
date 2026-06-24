using TransitNova.BusinessLayer.Common.CQRS;
using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Vehicle;
using TransitNova.BusinessLayer.Features.Vehicles.Queries;
using TransitNova.BusinessLayer.Interfaces.Repositories.VehicleRepository;

namespace TransitNova.BusinessLayer.Features.Vehicles.Handlers.ApplyQueries
{
    public sealed class GetActiveVehiclesHandler(
        IVehicleQueryRepository vehicleRepository,
        ILogger<GetActiveVehiclesHandler> logger)
        : IQueryHandler<GetActiveVehiclesQuery, Result<List<VehicleDto>>>
    {
        public async Task<Result<List<VehicleDto>>> Handle(GetActiveVehiclesQuery request, CancellationToken ct)
        {
            var vehicles = await vehicleRepository.GetActiveAsync(ct);
            if (vehicles.Count == 0)
            {
                logger.LogInformation("No active vehicles found.");
                return Result<List<VehicleDto>>.Success([]);
            }

            return Result<List<VehicleDto>>.Success(vehicles);
        }
    }
}
