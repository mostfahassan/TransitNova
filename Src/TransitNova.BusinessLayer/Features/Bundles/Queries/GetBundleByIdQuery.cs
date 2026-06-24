using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Bundle;

namespace TransitNova.BusinessLayer.Features.Bundles.Queries
{
    // --- Features/Bundle/Queries/GetBundleByIdQuery.cs ---
    public sealed record GetBundleByIdQuery(Guid Id)
        : IQuery<Result<RetrieveBundleDto?>>;

}
