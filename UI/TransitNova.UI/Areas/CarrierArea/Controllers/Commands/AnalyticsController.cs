using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransitNova.Domain.Contracts.Roles;
using TransitNova.UI.Infrastructure.Mvc.Common;
using TransitNova.UI.Infrastructure.Mvc.Interface;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Carrier.Analytics.Queries.Segregation;
namespace TransitNova.UI.Areas.CarrierArea.Controllers.Commands;

[Authorize(Roles = Role.Carrier)]
[Area("CarrierArea")]
[Route("[area]/[controller]/[action]")]
public sealed class AnalyticsController(
    IBackendApiInvoker apiInvoker,
    ICarrierAnalyticsQuery carrierAnalyticsQuery)
    : AppControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Rating(CancellationToken cancellationToken)
    {
        if (ResolvedCarrierId is not Guid carrierId)
            return Challenge();

        var response = await apiInvoker.ExecuteAsync((token, ct) => carrierAnalyticsQuery.GetCarrierRatingAsync(carrierId, token!, ct), cancellationToken: cancellationToken);

        return response.IsSuccess ? View(response.Data) : HandleGetFailure(response);
    }

    [HttpGet]
    public async Task<IActionResult> Revenue(CancellationToken cancellationToken)
    {
        if (ResolvedCarrierId is not Guid carrierId)
            return Challenge();

        var response = await apiInvoker.ExecuteAsync((token, ct) => carrierAnalyticsQuery.GetCarrierRevenueAsync(carrierId, token!, ct), cancellationToken: cancellationToken);

        return response.IsSuccess ? View(response.Data) : HandleGetFailure(response);
    }
}

