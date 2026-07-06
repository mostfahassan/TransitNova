using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransitNova.Domain.Contracts.Roles;
using TransitNova.UI.Infrastructure.Mvc;
 
 
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Carrier.Dashboard.Segregation;

namespace TransitNova.UI.Areas.CarrierArea.Controllers.Queries;

[Authorize(Roles = Role.Carrier)]
[Area("CarrierArea")]
[Route("[area]/[controller]/[action]")]
public sealed class DashboardController(
    IBackendApiInvoker apiInvoker,
    ICarrierDashboardQuery carrierDashboardQuery)
    : AppControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        if (ResolvedCarrierId is not Guid carrierId)
            return Challenge();

        var response = await apiInvoker.ExecuteAsync((token, ct) => carrierDashboardQuery.GetCarrierDashboardAsync(carrierId, token!, ct), cancellationToken: cancellationToken);

        if (response.IsFailure)
            return HandleGetFailure(response);

        if (response.Data?.Profile.Id is Guid profileCarrierId && profileCarrierId != Guid.Empty)
            SetCurrentCarrierId(profileCarrierId);

        return View(response.Data);
    }
}
