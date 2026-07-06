using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransitNova.Domain.Contracts.Roles;
using TransitNova.UI.ViewModels;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.WarehouseManagers.Segregations.Query;

namespace TransitNova.UI.Areas.AdminArea.Controllers.WarehouseManagers;

[Authorize(Roles = Role.Admin)]
[Area("AdminArea")]
[Route("[area]/[controller]/[action]")]
public sealed class WarehouseManagersController(
    IBackendApiInvoker apiInvoker,
    IAdminWarehouseManagersQuery adminWarehouseManagersQuery)
    : AppControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Index(WarehouseManagerFilterViewModel filter, CancellationToken cancellationToken)
    {
        var response = await apiInvoker.ExecuteAsync((token, ct) => adminWarehouseManagersQuery.GetWarehouseManagersAsync(filter.ToDto(), token!, ct), cancellationToken: cancellationToken);

        return response.IsSuccess ? View(response.Data) : HandleGetFailure(response);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Details(Guid id, CancellationToken cancellationToken)
    {
        var response = await apiInvoker.ExecuteAsync((token, ct) => adminWarehouseManagersQuery.GetWarehouseManagerByIdAsync(id, token!, ct), cancellationToken: cancellationToken);

        return response.IsSuccess ? View(response.Data) : HandleGetFailure(response);
    }
}
