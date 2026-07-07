using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransitNova.Domain.Contracts.Roles;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Subscriptions.Segregation;

namespace TransitNova.UI.Areas.AdminArea.Controllers.Subscriptions;

[Authorize(Roles = Role.Admin)]
[Area("AdminArea")]
[Route("[area]/[controller]/[action]")]
public sealed class SubscriptionsController(
    IBackendApiInvoker apiInvoker,
    IAdminSubscriptionQuery adminSubscriptionQuery)
    : AppControllerBase
{
    [HttpGet("{subscriptionId:guid}")]
    public async Task<IActionResult> Details(Guid subscriptionId, CancellationToken cancellationToken)
    {
        var response = await apiInvoker.ExecuteAsync((token, ct) => adminSubscriptionQuery.GetSubscriptionByIdAsync(subscriptionId, token!, ct), cancellationToken: cancellationToken);

        return response.IsSuccess ? View(response.Data) : HandleGetFailure(response);
    }

    [HttpGet("{bundleId:guid}")]
    public async Task<IActionResult> BundleSubscribers(Guid bundleId, CancellationToken cancellationToken)
    {
        var response = await apiInvoker.ExecuteAsync((token, ct) => adminSubscriptionQuery.GetBundleSubscribersAsync(bundleId, token!, ct), cancellationToken: cancellationToken);

        return response.IsSuccess ? View(response.Data) : HandleGetFailure(response);
    }
}