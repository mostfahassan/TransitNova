using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransitNova.Domain.Contracts.Roles;
using TransitNova.UI.Infrastructure.Mvc.Common;
using TransitNova.UI.Infrastructure.Mvc.Interface;
using TransitNova.UI.ViewModels;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Users.Segregation;

namespace TransitNova.UI.Areas.AdminArea.Controllers.Users;

[Authorize(Roles = Role.Admin)]
[Area("AdminArea")]
[Route("[area]/[controller]/[action]")]
public sealed class UsersController(
    IBackendApiInvoker apiInvoker,
    IAdminUserQuery adminUserQuery)
    : AppControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Index(UserFilterViewModel filter, CancellationToken cancellationToken)
    {
        var response = await apiInvoker.ExecuteAsync((token, ct) => adminUserQuery.FilterUsersAsync(filter.ToDto(), token!, ct), cancellationToken: cancellationToken);

        return response.IsSuccess ? View(response.Data) : HandleGetFailure(response);
    }

    [HttpGet("{userId:guid}")]
    public async Task<IActionResult> Details(Guid userId, CancellationToken cancellationToken)
    {
        var response = await apiInvoker.ExecuteAsync((token, ct) => adminUserQuery.GetUserDetailsAsync(userId, token!, ct), cancellationToken: cancellationToken);

        return response.IsSuccess ? View(response.Data) : HandleGetFailure(response);
    }
}
