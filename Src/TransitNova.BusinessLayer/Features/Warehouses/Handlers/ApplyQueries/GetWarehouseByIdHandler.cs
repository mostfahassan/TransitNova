using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Warehouse;
using TransitNova.BusinessLayer.Features.Warehouses.Queries;
using TransitNova.BusinessLayer.Interfaces.Repositories.WarehouseRepository;

namespace TransitNova.BusinessLayer.Features.Warehouses.Handlers.ApplyQueries
{
    public sealed class GetWarehouseByIdHandler(
        IWarehouseQueriesRepository warehouseRepository,
        ILogger<GetWarehouseByIdHandler> logger)
        : IQueryHandler<GetWarehouseByIdQuery, Result<WarehouseDto?>>
    {
        public async Task<Result<WarehouseDto?>> Handle(GetWarehouseByIdQuery request, CancellationToken ct)
        {
            if (request.WarehouseId == Guid.Empty)
                return Result<WarehouseDto?>.Failure(Errors.Validation("Warehouse id is required."));

            var warehouse = await warehouseRepository.GetWarehouseByIdAsync(request.WarehouseId, ct);
            if (warehouse is null)
            {
                logger.LogWarning("Warehouse not found. WarehouseId: {WarehouseId}", request.WarehouseId);
                return Result<WarehouseDto?>.NotFound(Errors.NotFound("Warehouse not found."));
            }

            return Result<WarehouseDto?>.Success(warehouse);
        }
    }
}
