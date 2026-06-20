using TransitNova.BusinessLayer.Common.Caching;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.CarrierCompany;
using TransitNova.BusinessLayer.Interfaces.MarkerInterfaces;

namespace TransitNova.BusinessLayer.Features.CarrierCompanies.Queries
{
    // --- Features/CarrierCompany/Queries/GetCarrierCompanyListQuery.cs ---
    public record GetCarrierCompanyListQuery()
        : IQuery<Result<List<RetrieveCarrierCompany>>>, ICachable
    {
        public string CacheKey => CacheKeys.CarrierCompanyList();
    }

}
