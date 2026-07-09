using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransitNova.Domain.Contracts.Roles;
using TransitNova.UI.Infrastructure.Mvc.Common;
using TransitNova.UI.Infrastructure.Mvc.Interface;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.OperationManagers.Segregation;

namespace TransitNova.UI.Areas.AdminArea.Controllers.OperationManagers;

[Authorize(Roles = Role.Admin)]
[Area("AdminArea")]
[Route("[area]/[controller]/[action]")]
public sealed class OperationManagersController(
    IBackendApiInvoker apiInvoker,
    IAdminOperationManagerQuery adminOperationManagerQuery)
    : AppControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var response = await apiInvoker.ExecuteAsync((token, ct) => adminOperationManagerQuery.GetOperationManagersAsync(token!, ct), cancellationToken: cancellationToken);

        return response.IsSuccess ? View(response.Data) : HandleGetFailure(response);
    }

    [HttpGet]
    public async Task<IActionResult> Active(CancellationToken cancellationToken)
    {
        var response = await apiInvoker.ExecuteAsync((token, ct) => adminOperationManagerQuery.GetActiveOperationManagersAsync(token!, ct), cancellationToken: cancellationToken);

        return response.IsSuccess ? View(response.Data) : HandleGetFailure(response);
    }

    [HttpGet("{operationManagerId:guid}")]
    public async Task<IActionResult> Details(Guid operationManagerId, CancellationToken cancellationToken)
    {
        var response = await apiInvoker.ExecuteAsync((token, ct) => adminOperationManagerQuery.GetOperationManagerByIdAsync(operationManagerId, token!, ct), cancellationToken: cancellationToken);

        return response.IsSuccess ? View(response.Data) : HandleGetFailure(response);
    }

    [HttpGet("{operationManagerId:guid}")]
    public async Task<IActionResult> HandledCarriers(Guid operationManagerId, int pageNumber = 1, int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var response = await apiInvoker.ExecuteAsync((token, ct) => adminOperationManagerQuery.GetHandledCarriersAsync(operationManagerId, token!, pageNumber, pageSize, ct), cancellationToken: cancellationToken);

        return response.IsSuccess ? View(response.Data) : HandleGetFailure(response);
    }

    [HttpGet("{operationManagerId:guid}")]
    public async Task<IActionResult> HandledShipments(Guid operationManagerId, int pageNumber = 1, int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var response = await apiInvoker.ExecuteAsync((token, ct) => adminOperationManagerQuery.GetHandledShipmentsAsync(operationManagerId, token!, pageNumber, pageSize, ct), cancellationToken: cancellationToken);

        return response.IsSuccess ? View(response.Data) : HandleGetFailure(response);
    }
}
