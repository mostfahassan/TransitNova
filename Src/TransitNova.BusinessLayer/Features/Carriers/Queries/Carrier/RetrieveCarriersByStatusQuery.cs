using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.Interfaces.MarkerInterfaces;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Carrier;
using TransitNova.Domain.Contracts.Caching;
using TransitNova.Domain.Enums.Carrier;
namespace TransitNova.BusinessLayer.Features.Carriers.Queries.Carrier
{
    public sealed record RetrieveCarriersByStatusQuery(CarrierStatus CarrierStatus)
        : IQuery<Result<IEnumerable<CarrierProfileDto>>>, ICachable
    {
        public string CacheKey => CacheKeys.Carriers.ByStatus(CarrierStatus);
    }

}
