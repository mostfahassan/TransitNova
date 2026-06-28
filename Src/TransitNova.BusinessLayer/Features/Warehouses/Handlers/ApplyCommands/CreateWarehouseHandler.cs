using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Warehouse;
using TransitNova.BusinessLayer.Features.Warehouses.Commands;
using TransitNova.BusinessLayer.Interfaces.Repositories.AdminRepository;
using TransitNova.BusinessLayer.Interfaces.Repositories.SystemLogRepository;
using TransitNova.BusinessLayer.Interfaces.Repositories.WarehouseRepository;
using TransitNova.BusinessLayer.Interfaces.Services.UnitOfWork;
using TransitNova.Domain.Entities.MainEntities;
using TransitNova.Domain.Enums.SystemLogs;
namespace TransitNova.BusinessLayer.Features.Warehouses.Handlers.ApplyCommands
{
    public sealed class CreateWarehouseHandler(
        IWarehouseCommandsRepository commandRepository,
        IWarehouseQueriesRepository queryRepository,
        IAdminQueryRepository adminQueryRepository,
        ISystemLogCommands systemLogCommands,
        IUnitOfWork unitOfWork,
        ILogger<CreateWarehouseHandler> logger)
        : ICommandHandler<CreateWarehouseCommand, Result<WarehouseDto>>
    {
        public async Task<Result<WarehouseDto>> Handle(CreateWarehouseCommand request, CancellationToken ct)
        {
            var zoneIds = request.Dto.ZoneIds.Distinct().ToList();
            var zones = zoneIds.Count == 0
                ? []
                : await queryRepository.GetZonesByIdsAsync(zoneIds, ct);

            if (zones.Count != zoneIds.Count)
            {
                return Result<WarehouseDto>.Validation(
                [
                    Errors.Validation("One or more warehouse zones were not found.")
                ]);
            }

            var warehouse = Warehouse.Create(
                request.Dto.Name,
                request.Dto.Type,
                request.Dto.Capacity,
                request.Dto.CurrentUsage,
                request.Dto.OperatingHours,
                request.Dto.Address,
                request.AdminId,
                request.Dto.ManagerId);

            foreach (var zone in zones)
            {
                warehouse.AddZone(zone);
            }

            await commandRepository.AddAsync(warehouse, ct);
            var performedByName = await adminQueryRepository.GetAdminNameAsync(request.AdminId, ct);
            var log = SystemActivityLog.AddLog(
                ActivityAction.Created,
                ActivityEntityType.Warehouse,
                $"Warehouse {warehouse.Id} ({warehouse.Name}) was created.",
                request.AdminId,
                performedByName);

            await systemLogCommands.LogAsync(log, ct);
            await unitOfWork.SaveChangesAsync(ct);

            var createdWarehouse = await queryRepository.GetWarehouseByIdAsync(warehouse.Id, ct);
            if (createdWarehouse is null)
            {
                logger.LogWarning("Warehouse created but could not be retrieved. WarehouseId: {WarehouseId}", warehouse.Id);
                return Result<WarehouseDto>.Failure(Errors.NotFound("Warehouse not found"));
            }

            logger.LogInformation("Warehouse created successfully. WarehouseId: {WarehouseId}", warehouse.Id);
            return Result<WarehouseDto>.Created(createdWarehouse);
        }
    }
}
