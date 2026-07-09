using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransitNova.Domain.Contracts.Roles;
using TransitNova.UI.Infrastructure.Mvc.Common;
using TransitNova.UI.Infrastructure.Mvc.Interface;
using TransitNova.UI.ViewModels;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.User.PaymentInvoices.Segregation;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.User.Profile.Segregation;

namespace TransitNova.UI.Areas.UserArea.Controllers;

[Authorize(Roles = Role.User)]
[Area("UserArea")]
[Route("[area]/[controller]/[action]")]
public sealed class DashboardController(
    IBackendApiInvoker apiInvoker,
    IUserProfileQuery userProfileQuery,
    IUserPaymentInvoicesQuery userPaymentInvoicesQuery)
    : AppControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var dashboardResponse = await apiInvoker.ExecuteAsync((token, ct) => userProfileQuery.GetUserDashboardAsync(token!, ct), cancellationToken: cancellationToken);

        if (dashboardResponse.IsFailure)
            return HandleGetFailure(dashboardResponse);

        var invoicesResponse = await apiInvoker.ExecuteAsync((token, ct) => userPaymentInvoicesQuery.GetUserPaymentInvoicesAsync(token!, ct), cancellationToken: cancellationToken);
        var invoices = invoicesResponse.IsSuccess
            ? invoicesResponse.Data?.OrderByDescending(invoice => invoice.PaidAt ?? DateTime.MinValue).ToArray() ?? []
            : [];

        if (invoicesResponse.IsFailure)
            ApiError(invoicesResponse);

        return View(new UserDashboardPageViewModel(dashboardResponse.Data, invoices.Take(5).ToArray(), invoices.Length));
    }
}
