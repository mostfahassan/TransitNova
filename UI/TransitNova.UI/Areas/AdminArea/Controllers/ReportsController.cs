using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransitNova.Domain.Contracts.Roles;
using TransitNova.UI.Infrastructure.Mvc.Common;
using TransitNova.UI.Infrastructure.Mvc.Interface;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Shared.Reports.Segregation;
using TransitNovaUI.BusinessLayer.DTOs.Reports;

namespace TransitNova.UI.Areas.AdminArea.Controllers;

[Authorize(Roles = Role.Admin)]
[Area("AdminArea")]
[Route("[area]/[controller]/[action]")]
public sealed class ReportsController(
    IBackendApiInvoker apiInvoker,
    IIdempotencyKeyFactory idempotencyKeyFactory,
    ISharedReportsCommand sharedReportsCommand)
    : AppControllerBase
{
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RequestDashboard(CancellationToken cancellationToken)
    {
        var response = await apiInvoker.ExecuteAsync(
            (token, ct) => sharedReportsCommand.RequestDashboardReportAsync(
                new UiDashboardReportContract(),
                token!,
                idempotencyKeyFactory.Create(),
                ct),
            cancellationToken: cancellationToken);

        if (response.IsFailure)
            ApiError(response);
        else
            Success(response.Message ?? "Dashboard report requested successfully.");

        return RedirectToAction("Index", "Dashboard", new { area = "AdminArea" });
    }
}