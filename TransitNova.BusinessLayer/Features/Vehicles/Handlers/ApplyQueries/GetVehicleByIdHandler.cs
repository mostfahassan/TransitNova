using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Vehicle;
using TransitNova.BusinessLayer.Features.Vehicles.Queries;
using TransitNova.BusinessLayer.Interfaces.Repositories.VehicleRepository;
namespace TransitNova.BusinessLayer.Features.Vehicles.Handlers.ApplyQueries
{
    public sealed class GetVehicleByIdHandler(
        IVehicleQueryRepository vehicleRepository,
        ILogger<GetVehicleByIdHandler> logger)
        : IQueryHandler<GetVehicleByIdQuery, Result<VehicleDto?>>
    {
        public async Task<Result<VehicleDto?>> Handle(GetVehicleByIdQuery request, CancellationToken ct)
        {
            if (request.Id == Guid.Empty)
                return Result<VehicleDto?>.Failure(Errors.Validation("Plate number is required."));

            var vehicle = await vehicleRepository.GetVehicleDetailsAsync(request.Id, ct);
            if (vehicle is null)
            {
                logger.LogWarning("Vehicle not found. VehicleId: {VehicleId}", request.Id);
                return Result<VehicleDto?>.NotFound(Errors.NotFound("Vehicle not found."));
            }

            return Result<VehicleDto?>.Success(vehicle);
        }
    }
}
