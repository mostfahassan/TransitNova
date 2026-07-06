using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransitNova.Domain.Contracts.Roles;
using TransitNova.UI.Infrastructure.Mvc;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.OperationManager.Dashboard.Segregation;

namespace TransitNova.UI.Areas.OperationManagerArea.Controllers.Queries;

[Authorize(Roles = Role.OperationManager)]
[Area("OperationManagerArea")]
[Route("[area]/[controller]/[action]")]
public sealed class DashboardController(
    IBackendApiInvoker apiInvoker,
    IOperationManagerDashboardQuery operationManagerDashboardQuery)
    : AppControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var response = await apiInvoker.ExecuteAsync((token, ct) => operationManagerDashboardQuery.GetOperationManagerDashboardAsync(token!, ct), cancellationToken: cancellationToken);

        return response.IsSuccess ? View(response.Data) : HandleGetFailure(response);
    }
}
