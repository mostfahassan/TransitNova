
using TransitNova.BusinessLayer.Common.Interfaces;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.CarrierCompany;
namespace TransitNova.BusinessLayer.Features.CarrierCompanies.Commands
{
    public record CreateCarrierCompanyCommand(Guid RequestId ,AddCarrierCompany Dto,Guid AdminId)
     : IdempotantCommand<Result<RetrieveCarrierCompany>>(RequestId);

}
