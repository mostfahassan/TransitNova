using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.Interfaces.MarkerInterfaces;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Carrier;
using TransitNova.Domain.Contracts.Caching;
namespace TransitNova.BusinessLayer.Features.Carriers.Queries.Carrier
{
    public sealed record GetCarrierDashboardQuery(Guid carrierId)
        : IQuery<Result<CarrierDashboardDto>>, ICachable
    {
        public string CacheKey => CacheKeys.Carriers.Dashboard(carrierId);
    }
}

