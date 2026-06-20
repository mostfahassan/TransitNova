using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Country;
using TransitNova.BusinessLayer.Features.Location.Governments.Queries;
using TransitNova.BusinessLayer.Interfaces.Repositories.GenericRepository;
using TransitNova.Domain.Entities.MainEntities;

namespace TransitNova.BusinessLayer.Features.Location.Governments.Handlers.ApplyingQueries
{
    public sealed class GetGovernmentsHandler(IGenericRepository<Government, int> repository)
        : IQueryHandler<GetGovernmentsQuery, Result<List<GovernmentDto>>>
    {
        public async Task<Result<List<GovernmentDto>>> Handle(GetGovernmentsQuery request, CancellationToken ct)
        {
            var governments = await repository.GetListAsync<GovernmentDto>(ct);
            return Result<List<GovernmentDto>>.Success(governments);
        }
    }
}
