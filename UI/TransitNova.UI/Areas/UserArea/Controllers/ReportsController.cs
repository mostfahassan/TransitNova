using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransitNova.Domain.Contracts.Roles;
using TransitNova.UI.Infrastructure.Mvc.Common;
using TransitNova.UI.Infrastructure.Mvc.Interface;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Shared.Reports.Segregation;
using TransitNovaUI.BusinessLayer.DTOs.Reports;

namespace TransitNova.UI.Areas.UserArea.Controllers;

[Authorize(Roles = Role.User)]
[Area("UserArea")]
[Route("[area]/[controller]/[action]")]
public sealed class ReportsController(
    IBackendApiInvoker apiInvoker,
    IIdempotencyKeyFactory idempotencyKeyFactory,
    ISharedReportsCommand sharedReportsCommand)
    : AppControllerBase
{
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RequestBundleInvoice(Guid paymentId, CancellationToken cancellationToken)
    {
        if (paymentId == Guid.Empty)
        {
            return BadRequest(new { isSuccess = false, message = "Invalid payment reference." });
        }

        var response = await apiInvoker.ExecuteAsync(
            (token, ct) => sharedReportsCommand.RequestBundleReportAsync(
                new UiBundleReportContract { PaymentId = paymentId },
                token!,
                idempotencyKeyFactory.Create(),
                ct),
            cancellationToken: cancellationToken);

        if (response.IsFailure)
        {
            return StatusCode(
                response.StatusCode >= StatusCodes.Status400BadRequest ? response.StatusCode : StatusCodes.Status400BadRequest,
                new
                {
                    isSuccess = false,
                    message = response.Error?.Message ?? response.Message ?? "The bundle invoice report request could not be completed."
                });
        }

        return Ok(new
        {
            isSuccess = true,
            message = response.Message ?? "Bundle invoice report requested successfully."
        });
    }
}