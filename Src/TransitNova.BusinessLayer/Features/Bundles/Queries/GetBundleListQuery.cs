using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.Interfaces.MarkerInterfaces;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Bundle;
using TransitNova.Domain.Contracts.Caching;
namespace TransitNova.BusinessLayer.Features.Bundles.Queries
{
    public sealed record GetBundleListQuery() : IQuery<Result<List<RetrieveBundleDto>>>, ICachable
    {
        public string CacheKey => CacheKeys.Bundles.List;
    }
}

