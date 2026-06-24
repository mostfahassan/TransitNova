using TransitNova.BusinessLayer.Common.CQRS;
using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.DTOs.Bundle;
using TransitNova.BusinessLayer.Features.Bundles.Queries;
using TransitNova.Domain.Entities.MainEntities;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.Interfaces.Repositories.GenericRepository;
namespace TransitNova.BusinessLayer.Features.Bundles.Handlers.ApplyingQueries
{
    public sealed class GetBundleListHandler(
        IGenericRepository<Bundle, Guid> repository,
        ILogger<GetBundleListHandler> logger)
        : IQueryHandler<GetBundleListQuery, Result<List<RetrieveBundleDto>>>
    {
        public async Task<Result<List<RetrieveBundleDto>>> Handle(GetBundleListQuery request, CancellationToken ct)
        {
            var list = await repository.GetListAsync<RetrieveBundleDto>(ct);

            if (list.Count == 0)
            {
                logger.LogInformation("No bundles found.");
                var emptyResult = Result<List<RetrieveBundleDto>>.Success([]);
                return emptyResult;
            }

            var result = Result<List<RetrieveBundleDto>>.Success(list);
            return result;
        }
    }

}
