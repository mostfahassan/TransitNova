using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransitNova.Domain.Contracts.Roles;
using TransitNova.UI.Infrastructure.Mvc.Common;
using TransitNova.UI.Infrastructure.Mvc.Interface;
using TransitNova.UI.ViewModels;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Trips.Segregation;

namespace TransitNova.UI.Areas.AdminArea.Controllers.Trips;

[Authorize(Roles = Role.Admin)]
[Area("AdminArea")]
[Route("[area]/[controller]/[action]")]
public sealed class TripsController(
    IBackendApiInvoker apiInvoker,
    IAdminTripsQuery adminTripsQuery)
    : AppControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Index(TripFilterViewModel filter, CancellationToken cancellationToken)
    {
        var response = await apiInvoker.ExecuteAsync((token, ct) => adminTripsQuery.GetAdminTripsAsync(filter.ToDto(), token!, ct), cancellationToken: cancellationToken);
        return response.IsSuccess ? View(response.Data) : HandleGetFailure(response);
    }

    [HttpGet("{tripId:guid}")]
    public async Task<IActionResult> Details(Guid tripId, CancellationToken cancellationToken)
    {
        var response = await apiInvoker.ExecuteAsync((token, ct) => adminTripsQuery.GetAdminTripByIdAsync(tripId, token!, ct), cancellationToken: cancellationToken);
        return response.IsSuccess ? View(response.Data) : HandleGetFailure(response);
    }
}