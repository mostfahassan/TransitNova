using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransitNova.Domain.Contracts.Roles;
using TransitNova.UI.Infrastructure.Mvc.Common;
using TransitNova.UI.Infrastructure.Mvc.Interface;
using TransitNovaUI.BusinessLayer.DTOs.Payment;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.PaymentHistories.Segregation;

namespace TransitNova.UI.Areas.AdminArea.Controllers.PaymentHistories;

[Authorize(Roles = Role.Admin)]
[Area("AdminArea")]
[Route("[area]/[controller]/[action]")]
public sealed class PaymentHistoriesController(
    IBackendApiInvoker apiInvoker,
    IAdminPaymentHistoriesQuery adminPaymentHistoriesQuery)
    : AppControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] UiPaymentHistoryFilterDto filter, CancellationToken cancellationToken)
    {
        var response = await apiInvoker.ExecuteAsync(
            (token, ct) => adminPaymentHistoriesQuery.GetAdminPaymentHistoriesAsync(filter, token!, ct),
            cancellationToken: cancellationToken);

        return response.IsSuccess ? View(response.Data) : HandleGetFailure(response);
    }
}

