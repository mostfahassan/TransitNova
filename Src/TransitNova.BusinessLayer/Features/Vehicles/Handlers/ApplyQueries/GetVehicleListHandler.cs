using TransitNova.BusinessLayer.Common.CQRS;
using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Vehicle;
using TransitNova.BusinessLayer.Features.Vehicles.Queries;
using TransitNova.BusinessLayer.Interfaces.Repositories.VehicleRepository;
namespace TransitNova.BusinessLayer.Features.Vehicles.Handlers.ApplyQueries
{
    public sealed class GetVehicleListHandler(
        IVehicleQueryRepository vehicleRepository,
        ILogger<GetVehicleListHandler> logger)
        : IQueryHandler<GetVehicleListQuery, Result<List<VehicleDto>>>
    {
        public async Task<Result<List<VehicleDto>>> Handle(GetVehicleListQuery request, CancellationToken ct)
        {
            var vehicles = await vehicleRepository.GetListAsync<VehicleDto>(ct);
            if (vehicles.Count == 0)
            {
                logger.LogInformation("No vehicles found.");
                return Result<List<VehicleDto>>.Success([]);
            }

            return Result<List<VehicleDto>>.Success(vehicles);
        }
    }
}
