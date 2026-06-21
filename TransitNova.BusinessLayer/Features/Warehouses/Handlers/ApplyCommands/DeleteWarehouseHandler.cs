using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.Features.Warehouses.Commands;
using TransitNova.BusinessLayer.Interfaces.MarkerInterfaces;
using TransitNova.BusinessLayer.Interfaces.Repositories.Admin;
using TransitNova.BusinessLayer.Interfaces.Repositories.SystemLogRepository;
using TransitNova.BusinessLayer.Interfaces.Repositories.WarehouseRepository;
using TransitNova.BusinessLayer.Interfaces.Services.UnitOfWork;

using TransitNova.Domain.Entities.MainEntities;
using TransitNova.Domain.Enums.SystemLogs;

namespace TransitNova.BusinessLayer.Features.Warehouses.Handlers.ApplyCommands
{
    public sealed class DeleteWarehouseHandler(
        IWarehouseCommandsRepository commandRepository,
        IAdminQueryRepository adminQueryRepository,
        ISystemLogCommands systemLogCommands,
        IUnitOfWork unitOfWork,
        ILogger<DeleteWarehouseHandler> logger)
        : ICommandHandler<DeleteWarehouseCommand, BaseResult>
    {
        public async Task<BaseResult> Handle(DeleteWarehouseCommand request, CancellationToken ct)
        {
            var deleted = await commandRepository.DeleteAsync(request.WarehouseId, ct);
            if (!deleted)
            {
                logger.LogWarning("Warehouse not found. WarehouseId: {WarehouseId}", request.WarehouseId);
                return BaseResult.NotFound(Errors.NotFound("Warehouse not found."));
            }

            var performedByName = await adminQueryRepository.GetAdminNameAsync(request.AdminId, ct);
            var log = SystemActivityLog.AddLog(
                ActivityAction.Deleted,
                ActivityEntityType.Warehouse,
                $"Warehouse {request.WarehouseId} was deleted.",
                request.AdminId,
                performedByName);

            await systemLogCommands.Log(log, ct);
            await unitOfWork.SaveChangesAsync(ct);
            logger.LogInformation("Warehouse deleted successfully. WarehouseId: {WarehouseId}", request.WarehouseId);
            return BaseResult.Success();
        }
    }
}
