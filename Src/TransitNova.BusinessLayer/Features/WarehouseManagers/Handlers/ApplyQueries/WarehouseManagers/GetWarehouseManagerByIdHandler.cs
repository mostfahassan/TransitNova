using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.WarehouseManager;
using TransitNova.BusinessLayer.Features.WarehouseManagers.Queries;
using TransitNova.BusinessLayer.Interfaces.Repositories.WarehouseManagerRepository;

namespace TransitNova.BusinessLayer.Features.WarehouseManagers.Handlers.ApplyQueries.WarehouseManagers
{
    public sealed class GetWarehouseManagerByIdHandler(IWarehouseManagerQueryRepository queryRepository)
        : IQueryHandler<GetWarehouseManagerByIdQuery, Result<WarehouseManagerDetailsDto>>
    {
        public async Task<Result<WarehouseManagerDetailsDto>> Handle(GetWarehouseManagerByIdQuery request, CancellationToken ct)
        {
            var manager = await queryRepository.GetByIdAsync(request.ManagerId, ct);

            return manager is null
                ? Result<WarehouseManagerDetailsDto>.NotFound(Errors.NotFound("Warehouse manager not found."))
                : Result<WarehouseManagerDetailsDto>.Success(manager);
        }
    }
}
