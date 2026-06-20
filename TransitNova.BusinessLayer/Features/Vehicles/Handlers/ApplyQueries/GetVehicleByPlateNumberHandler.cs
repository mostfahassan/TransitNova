using TransitNova.BusinessLayer.Common.CQRS;
using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Vehicle;
using TransitNova.BusinessLayer.Features.Vehicles.Queries;
using TransitNova.BusinessLayer.Interfaces.Repositories.VehicleRepository;
namespace TransitNova.BusinessLayer.Features.Vehicles.Handlers.ApplyQueries
{
    public sealed class GetVehicleByPlateNumberHandler(
        IVehicleQueryRepository vehicleRepository,
        ILogger<GetVehicleByPlateNumberHandler> logger)
        : IQueryHandler<GetVehicleByPlateNumberQuery, Result<VehicleDto?>>
    {
        public async Task<Result<VehicleDto?>> Handle(GetVehicleByPlateNumberQuery request, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(request.PlateNumber)) return Result<VehicleDto?>.Failure(Errors.Validation("Plate number is required."));

            var plateNumber = request.PlateNumber.Trim();
            var vehicle = await vehicleRepository.GetByPlateNumberAsync(plateNumber, ct);
            if (vehicle is null)
            {
                logger.LogWarning("Vehicle not found. PlateNumber: {PlateNumber}", plateNumber);
                return Result<VehicleDto?>.NotFound(Errors.NotFound("Vehicle not found."));
            }

            return Result<VehicleDto?>.Success(vehicle);
        }
    }
}
