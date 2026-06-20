
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;

namespace TransitNova.BusinessLayer.Features.Carriers.Queries
{
    public record GetCarrierRatingQuery(Guid CarrierId) : IQuery<Result<decimal>>;
}