using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransitNova.Domain.Contracts.Roles;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Shared.Notifications.Segregation;

namespace TransitNova.UI.Areas.UserArea.Controllers;

[Authorize(Roles = Role.User)]
[Area("UserArea")]
[Route("[area]/[controller]/[action]")]
public sealed class NotificationsController(
    IBackendApiInvoker apiInvoker,
    ISharedNotificationsQuery notificationsQuery,
    ISharedNotificationsCommand notificationsCommand)
    : AppControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var response = await apiInvoker.ExecuteAsync(
            (token, ct) => notificationsQuery.GetNotificationsAsync(pageNumber, pageSize, token!, ct),
            cancellationToken: cancellationToken);

        if (response.IsFailure)
            return HandleGetFailure(response);

        var markAllResponse = await apiInvoker.ExecuteAsync(
            (token, ct) => notificationsCommand.MarkAllAsReadAsync(token!, ct),
            cancellationToken: cancellationToken);

        if (markAllResponse.IsSuccess && response.Data is not null)
        {
            foreach (var notification in response.Data.Data)
            {
                notification.IsRead = true;
            }
        }

        return View(response.Data);
    }
}