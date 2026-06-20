using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.CarrierCompany;

namespace TransitNova.BusinessLayer.Features.CarrierCompanies.Queries
{
    // --- Features/CarrierCompany/Queries/GetCarrierCompanyByIdQuery.cs ---
    public record GetCarrierCompanyByIdQuery(Guid Id)
        : IQuery<Result<RetrieveCarrierCompany?>>;

}
