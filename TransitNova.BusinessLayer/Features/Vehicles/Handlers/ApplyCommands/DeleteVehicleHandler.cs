using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.Features.Vehicles.Commands;
using TransitNova.BusinessLayer.Interfaces.Repositories.VehicleRepository;
using TransitNova.BusinessLayer.Interfaces.Services.UnitOfWork;
 

namespace TransitNova.BusinessLayer.Features.Vehicles.Handlers.ApplyCommands
{
    public sealed class DeleteVehicleHandler(
        IVehicleQueryRepository vehicleRepository,
        IUnitOfWork unitOfWork,
        ILogger<DeleteVehicleHandler> logger)
        : ICommandHandler<DeleteVehicleCommand, BaseResult>
    {
        public async Task<BaseResult> Handle(DeleteVehicleCommand request, CancellationToken ct)
        {
            await vehicleRepository.DeleteAsync(request.Id, ct);
            await unitOfWork.SaveChangesAsync(ct);

            logger.LogInformation("Vehicle deleted successfully. VehicleId: {VehicleId}", request.Id);
            return BaseResult.Success();
        }
    }
}
