using TransitNova.BusinessLayer.Common.Interfaces;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Vehicle;

namespace TransitNova.BusinessLayer.Features.Vehicles.Commands
{
    public sealed record CreateVehicleCommand(Guid RequestId, VehicleDto Dto)
        : IdempotantCommand<Result<VehicleDto>>(RequestId);
}
