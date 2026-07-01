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
        : IQueryHandler<GetCarrierVehicleQuery, Result<VehicleDto?>>
    {
        public async Task<Result<VehicleDto?>> Handle(GetCarrierVehicleQuery request, CancellationToken ct)
        {
            if (request.CarrierId == Guid.Empty)
                return Result<VehicleDto?>.NotFound(Errors.CarrierNotFound("Carrier id is required."));

            var vehicle = await vehicleRepository.GetByCarrierIdAsync(request.CarrierId, ct);
            if (vehicle == null)
            {
                logger.LogInformation("No vehicle found for Carrier {UserId}", request.CarrierId);
                return Result<VehicleDto?>.NotFound(Errors.VehicleNotFound("Vehicle not found.Or Carrier Has No Vehicles."));
            }

            return Result<VehicleDto?>.Success(vehicle);
        }
    }
}
