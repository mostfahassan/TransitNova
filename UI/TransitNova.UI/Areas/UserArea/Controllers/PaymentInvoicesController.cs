using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransitNova.Domain.Contracts.Roles;
using TransitNova.UI.Infrastructure.Mvc.Common;
using TransitNova.UI.Infrastructure.Mvc.Interface;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.User.PaymentInvoices.Segregation;

namespace TransitNova.UI.Areas.UserArea.Controllers;

[Authorize(Roles = Role.User)]
[Area("UserArea")]
[Route("[area]/[controller]/[action]")]
public sealed class PaymentInvoicesController(
    IBackendApiInvoker apiInvoker,
    IUserPaymentInvoicesQuery userPaymentInvoicesQuery)
    : AppControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var response = await apiInvoker.ExecuteAsync((token, ct) => userPaymentInvoicesQuery.GetUserPaymentInvoicesAsync(token!, ct), cancellationToken: cancellationToken);

        if (response.IsFailure)
            return HandleGetFailure(response);

        var invoices = response.Data?.ToArray() ?? [];
        return View(invoices);
    }

    [HttpGet("{paymentId:guid}")]
    public async Task<IActionResult> Details(Guid paymentId, CancellationToken cancellationToken)
    {
        var response = await apiInvoker.ExecuteAsync((token, ct) => userPaymentInvoicesQuery.GetUserPaymentInvoiceAsync(paymentId, token!, ct), cancellationToken: cancellationToken);

        if (response.IsFailure)
            return HandleGetFailure(response);

        return response.Data is null
            ? RedirectToAction("NotFound", "Errors", new { area = "AccountArea" })
            : View(response.Data);
    }
}

