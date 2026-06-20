using TransitNova.BusinessLayer.Common.Caching;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Bundle;
using TransitNova.BusinessLayer.Interfaces.MarkerInterfaces;
namespace TransitNova.BusinessLayer.Features.Bundles.Queries
{
    public sealed record GetBundleListQuery() : IQuery<Result<List<RetrieveBundleDto>>>, ICachable
    {
        public string CacheKey => CacheKeys.BundleList();
    }
}
