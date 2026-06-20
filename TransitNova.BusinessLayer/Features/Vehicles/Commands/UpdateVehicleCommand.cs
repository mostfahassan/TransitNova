using TransitNova.BusinessLayer.Common.Interfaces;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Vehicle;

namespace TransitNova.BusinessLayer.Features.Vehicles.Commands
{
    public sealed record UpdateVehicleCommand(Guid RequestId, Guid Id, VehicleDto Dto)
        : IdempotantCommand<BaseResult>(RequestId);
}
