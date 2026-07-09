using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransitNova.Domain.Contracts.Roles;
using TransitNova.UI.Infrastructure.Mvc.Common;
using TransitNova.UI.Infrastructure.Mvc.Interface;
using TransitNova.UI.ViewModels;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Roles.Segregation;

namespace TransitNova.UI.Areas.AdminArea.Controllers.Roles;

[Authorize(Roles = Role.Admin)]
[Area("AdminArea")]
[Route("[area]/[controller]/[action]")]
public sealed class RolesController(
    IBackendApiInvoker apiInvoker,
    IIdempotencyKeyFactory idempotencyKeyFactory,
    IAdminRolesQuery adminRolesQuery,
    IAdminRolesCommand adminRolesCommand)
    : AppControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var response = await apiInvoker.ExecuteAsync((token, ct) => adminRolesQuery.GetRolesAsync(token!, ct), cancellationToken: cancellationToken);

        return response.IsSuccess ? View(response.Data) : HandleGetFailure(response);
    }

    [HttpGet("{roleId:guid}")]
    public async Task<IActionResult> Details(Guid roleId, CancellationToken cancellationToken)
    {
        var response = await apiInvoker.ExecuteAsync((token, ct) => adminRolesQuery.GetRoleByIdAsync(roleId, token!, ct), cancellationToken: cancellationToken);

        return response.IsSuccess ? View(response.Data) : HandleGetFailure(response);
    }

    [HttpGet]
    public IActionResult Create() => View(new RoleFormViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(RoleFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return View(model);

        var response = await apiInvoker.ExecuteAsync((token, ct) => adminRolesCommand.CreateRoleAsync(model.ToDto(), token!, idempotencyKeyFactory.Create(), ct), cancellationToken: cancellationToken);

        if (response.IsFailure)
        {
            AddApiErrors(response);
            return View(model);
        }

        Success("Role created successfully.");
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("{roleId:guid}")]
    public async Task<IActionResult> Edit(Guid roleId, CancellationToken cancellationToken)
    {
        var response = await apiInvoker.ExecuteAsync((token, ct) => adminRolesQuery.GetRoleByIdAsync(roleId, token!, ct), cancellationToken: cancellationToken);

        if (response.IsFailure)
            return HandleGetFailure(response);

        return response.Data is null
            ? RedirectToAction("NotFound", "Errors", new { area = "AccountArea" })
            : View(PrefillViewModelFactory.Role(response.Data));
    }

    [HttpPost("{roleId:guid}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid roleId, RoleFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return View(model);

        var response = await apiInvoker.ExecuteAsync((token, ct) => adminRolesCommand.UpdateRoleAsync(roleId, model.ToDto(), token!, idempotencyKeyFactory.Create(), ct), cancellationToken: cancellationToken);

        if (response.IsFailure)
        {
            AddApiErrors(response);
            return View(model);
        }

        Success("Role updated successfully.");
        return RedirectToAction(nameof(Details), new { roleId });
    }

    [HttpPost("{roleId:guid}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid roleId, CancellationToken cancellationToken)
    {
        var response = await apiInvoker.ExecuteAsync((token, ct) => adminRolesCommand.DeleteRoleAsync(roleId, token!, idempotencyKeyFactory.Create(), ct), cancellationToken: cancellationToken);

        if (response.IsFailure)
            ApiError(response);
        else
            Success("Role deleted successfully.");

        return RedirectToAction(nameof(Index));
    }

    [HttpGet("{roleId:guid}")]
    public async Task<IActionResult> Members(Guid roleId, CancellationToken cancellationToken)
    {
        var response = await apiInvoker.ExecuteAsync((token, ct) => adminRolesQuery.GetRoleMembersAsync(roleId, token!, ct), cancellationToken: cancellationToken);

        return response.IsSuccess ? View(response.Data) : HandleGetFailure(response);
    }

    [HttpPost("{roleId:guid}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Members(Guid roleId, RoleMembersFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return View(model);

        var response = await apiInvoker.ExecuteAsync((token, ct) => adminRolesCommand.UpdateRoleMembersAsync(roleId, model.ToDto(), token!, idempotencyKeyFactory.Create(), ct), cancellationToken: cancellationToken);

        if (response.IsFailure)
        {
            AddApiErrors(response);
            return View(model);
        }

        Success("Role members updated successfully.");
        return RedirectToAction(nameof(Members), new { roleId });
    }
}