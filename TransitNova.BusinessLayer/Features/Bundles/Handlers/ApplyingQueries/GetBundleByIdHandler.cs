using TransitNova.BusinessLayer.Common.CQRS;
using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.DTOs.Bundle;
using TransitNova.BusinessLayer.Features.Bundles.Queries;
using TransitNova.Domain.Entities.MainEntities;

using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.Interfaces.Repositories.GenericRepository;
namespace TransitNova.BusinessLayer.Features.Bundles.Handlers.ApplyingQueries
{
    public sealed class GetBundleByIdHandler(
    IGenericRepository<Bundle, int> repository,
    ILogger<GetBundleByIdHandler> logger)
    : IQueryHandler<GetBundleByIdQuery, Result<RetrieveBundleDto?>>
    {
        public async Task<Result<RetrieveBundleDto?>> Handle(GetBundleByIdQuery request, CancellationToken ct)
        {
            var dto = await repository.GetByIdAsync<RetrieveBundleDto>(request.Id, ct);
            if (dto is null)
            {
                logger.LogWarning("Bundle not found. Id: {BundleId}", request.Id);
                return Result<RetrieveBundleDto?>.NotFound(Errors.NotFound("Bundle not found"));
            }

            return Result<RetrieveBundleDto?>.Success(dto);
        }
    }
}
