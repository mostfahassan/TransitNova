using TransitNova.BusinessLayer.Common.Interfaces;
using System.Security.Claims;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.CarrierCompany;

namespace TransitNova.BusinessLayer.Features.CarrierCompanies.Commands
{
    // --- Features/CarrierCompany/Commands/UpdateCarrierCompanyCommand.cs ---
    public record UpdateCarrierCompanyCommand(Guid RequestId, Guid Id, UpdateCarrierCompany Dto, Guid AdminId)
        : IdempotantCommand<BaseResult>(RequestId);

}
