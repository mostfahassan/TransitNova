using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Country;

namespace TransitNova.BusinessLayer.Features.Location.Governments.Queries
{
    public sealed record GetGovernmentsQuery : IQuery<Result<List<GovernmentDto>>>;
}
