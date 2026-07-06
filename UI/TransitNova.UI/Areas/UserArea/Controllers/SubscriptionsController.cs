using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransitNova.Domain.Contracts.Roles;
 
 
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.User.Subscriptions.Segregation;

namespace TransitNova.UI.Areas.UserArea.Controllers;

[Authorize(Roles = Role.User)]
[Area("UserArea")]
[Route("[area]/[controller]/[action]")]
public sealed class SubscriptionsController(
    IBackendApiInvoker apiInvoker,
    IIdempotencyKeyFactory idempotencyKeyFactory,
    IUserSubscriptionCommand userSubscriptionCommand)
    : AppControllerBase
{
    [HttpPost("{bundleId:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Subscribe(int bundleId, CancellationToken cancellationToken)
    {
        var response = await apiInvoker.ExecuteAsync((token, ct) => userSubscriptionCommand.SubscribeToBundleAsync(bundleId, token!, idempotencyKeyFactory.Create(), ct), cancellationToken: cancellationToken);

        if (response.IsFailure)
            ApiError(response);
        else
            Success("Subscription completed successfully.");

        return RedirectToAction("Index", "Profile", new { area = "UserArea" });
    }

    [HttpPost("{bundleId:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Unsubscribe(int bundleId, CancellationToken cancellationToken)
    {
        var response = await apiInvoker.ExecuteAsync((token, ct) => userSubscriptionCommand.UnsubscribeFromBundleAsync(bundleId, token!, idempotencyKeyFactory.Create(), ct), cancellationToken: cancellationToken);

        if (response.IsFailure)
            ApiError(response);
        else
            Success("Subscription cancelled successfully.");

        return RedirectToAction("Index", "Profile", new { area = "UserArea" });
    }
}
