using TransitNova.BusinessLayer.Common.Interfaces;
using TransitNova.BusinessLayer.Common.ResultPattern;

namespace TransitNova.BusinessLayer.Features.Vehicles.Commands
{
    public sealed record DeleteVehicleCommand(Guid RequestId, Guid Id)
        : IdempotantCommand<BaseResult>(RequestId);
}
