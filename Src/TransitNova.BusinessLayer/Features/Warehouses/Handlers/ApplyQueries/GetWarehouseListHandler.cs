using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Warehouse;
using TransitNova.BusinessLayer.Features.Warehouses.Queries;
using TransitNova.BusinessLayer.Interfaces.Repositories.WarehouseRepository;

namespace TransitNova.BusinessLayer.Features.Warehouses.Handlers.ApplyQueries
{
    public sealed class GetWarehouseListHandler(
        IWarehouseQueriesRepository warehouseRepository,
        ILogger<GetWarehouseListHandler> logger)
        : IQueryHandler<GetWarehouseListQuery, Result<List<WarehouseDto>>>
    {
        public async Task<Result<List<WarehouseDto>>> Handle(GetWarehouseListQuery request, CancellationToken ct)
        {
            var warehouses = await warehouseRepository.GetWarehousesAsync(ct);
            if (warehouses.Count == 0)
            {
                logger.LogInformation("No warehouses found.");
                return Result<List<WarehouseDto>>.Success([]);
            }

            return Result<List<WarehouseDto>>.Success(warehouses);
        }
    }
}
