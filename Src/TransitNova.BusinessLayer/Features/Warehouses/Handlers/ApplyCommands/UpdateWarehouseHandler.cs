using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.Caching;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.Features.Warehouses.Commands;
using TransitNova.BusinessLayer.Interfaces.Repositories.AdminRepository;
using TransitNova.BusinessLayer.Interfaces.Repositories.SystemLogRepository;
using TransitNova.BusinessLayer.Interfaces.Repositories.WarehouseRepository;
using TransitNova.BusinessLayer.Interfaces.Services.UnitOfWork;
using TransitNova.Domain.Contracts.Caching;
using TransitNova.Domain.Entities.MainEntities;
using TransitNova.Domain.Enums.SystemLogs;
namespace TransitNova.BusinessLayer.Features.Warehouses.Handlers.ApplyCommands
{
    public sealed class UpdateWarehouseHandler(
        IWarehouseCommandsRepository commandRepository,
        IWarehouseQueriesRepository queryRepository,
        IAdminQueryRepository adminQueryRepository,
        ISystemLogCommands systemLogCommands,
        IUnitOfWork unitOfWork,
        ILogger<UpdateWarehouseHandler> logger)
        : ICommandHandler<UpdateWarehouseCommand, BaseResult>
    {
        public async Task<BaseResult> Handle(UpdateWarehouseCommand request, CancellationToken ct)
        {
            var warehouse = await queryRepository.GetWarehouseForUpdateAsync(request.WarehouseId, ct);
            if (warehouse is null)
            {
                logger.LogWarning("Warehouse not found. WarehouseId: {WarehouseId}", request.WarehouseId);
                return BaseResult.NotFound(Errors.NotFound("Warehouse not found."));
            }

            var zoneIds = request.Dto.ZoneIds.Distinct().ToList();
            var zones = zoneIds.Count == 0
                ? []
                : await queryRepository.GetZonesByIdsAsync(zoneIds, ct);

            if (zones.Count != zoneIds.Count)
            {
                return BaseResult.Validation(
                [
                    Errors.Validation("One or more warehouse zones were not found.")
                ]);
            }

            warehouse.Update(
                request.AdminId,
                request.Dto.Name,
                request.Dto.Type,
                request.Dto.Capacity,
                request.Dto.CurrentUsage,
                request.Dto.OperatingHours,
                request.Dto.Address?.ToDomain(),
                request.Dto.ManagerId);

            warehouse.ReplaceZones(zones, request.AdminId);

            commandRepository.Update(warehouse);
            var performedByName = await adminQueryRepository.GetAdminNameAsync(request.AdminId, ct);
            var log = SystemActivityLog.AddLog(
                ActivityAction.Updated,
                ActivityEntityType.Warehouse,
                $"Warehouse {warehouse.Id} ({warehouse.Name}) was updated.",
                request.AdminId,
                performedByName);

            await systemLogCommands.LogAsync(log, ct);
            await unitOfWork.SaveChangesAsync(ct);

            logger.LogInformation("Warehouse updated successfully. WarehouseId: {WarehouseId}", request.WarehouseId);

            CacheInvalidationContext.Set(request, CacheKeys.Warehouse.List,CacheKeys.Warehouse.ById(request.WarehouseId));

            return BaseResult.NoContent();
        }
    }
}

