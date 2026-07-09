using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransitNova.Domain.Contracts.Roles;
using TransitNova.UI.Infrastructure.Mvc.Common;
using TransitNova.UI.Infrastructure.Mvc.Interface;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Shared.Notifications.Segregation;

namespace TransitNova.UI.Areas.AdminArea.Controllers.Notifications;

[Authorize(Roles = Role.Admin)]
[Area("AdminArea")]
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