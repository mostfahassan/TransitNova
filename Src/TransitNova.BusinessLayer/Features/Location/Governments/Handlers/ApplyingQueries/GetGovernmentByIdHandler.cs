using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Country;
using TransitNova.BusinessLayer.Features.Location.Governments.Queries;
using TransitNova.BusinessLayer.Interfaces.Repositories.GenericRepository;
using TransitNova.Domain.Entities.MainEntities;

namespace TransitNova.BusinessLayer.Features.Location.Governments.Handlers.ApplyingQueries
{
    public sealed class GetGovernmentByIdHandler(IGenericRepository<Government, int> repository)
        : IQueryHandler<GetGovernmentByIdQuery, Result<GovernmentDto?>>
    {
        public async Task<Result<GovernmentDto?>> Handle(GetGovernmentByIdQuery request, CancellationToken ct)
        {
            var government = await repository.GetByIdAsync<GovernmentDto>(request.Id, ct);
            return government is null
                ? Result<GovernmentDto?>.NotFound(Errors.NotFound("Government not found."))
                : Result<GovernmentDto?>.Success(government);
        }
    }
}
