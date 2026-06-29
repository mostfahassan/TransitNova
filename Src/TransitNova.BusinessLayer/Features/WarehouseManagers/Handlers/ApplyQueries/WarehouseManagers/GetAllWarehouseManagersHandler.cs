using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.WarehouseManager;
using TransitNova.BusinessLayer.Features.WarehouseManagers.Queries;
using TransitNova.BusinessLayer.Interfaces.Repositories.WarehouseManagerRepository;

namespace TransitNova.BusinessLayer.Features.WarehouseManagers.Handlers.ApplyQueries.WarehouseManagers
{
    public sealed class GetAllWarehouseManagersHandler(IWarehouseManagerQueryRepository queryRepository)
        : IQueryHandler<GetAllWarehouseManagersQuery, Result<PagedResult<WarehouseManagerListDto>>>
    {
        public async Task<Result<PagedResult<WarehouseManagerListDto>>> Handle(GetAllWarehouseManagersQuery request, CancellationToken ct)
        {
            var managers = await queryRepository.GetAllWarehousesAsync(request.Filter, ct);
            return Result<PagedResult<WarehouseManagerListDto>>.Success(managers);
        }
    }
}
