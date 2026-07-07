using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransitNova.Domain.Contracts.Roles;
using TransitNova.UI.ViewModels;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Shipment.Segregation;

namespace TransitNova.UI.Areas.AdminArea.Controllers.Shipments
{
    [Authorize(Roles = Role.Admin)]
    [Area("AdminArea")]
    [Route("[area]/[controller]/[action]")]
    public class ShipmentsController(IBackendApiInvoker apiInvoker, IAdminShipmentsQuery adminShipmentsQuery) : AppControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] ShipmentFilterViewModel filter, CancellationToken cancellationToken)
        {
            var response = await apiInvoker.ExecuteAsync((token, ct) => adminShipmentsQuery.GetAdminShipmentsAsync(filter.ToDto(), token!, ct), cancellationToken: cancellationToken);

            return response.IsSuccess ? View(response.Data) : HandleGetFailure(response);
        }

        [HttpGet("{shipmentId:guid}")]
        public async Task<IActionResult> Details(Guid shipmentId, CancellationToken cancellationToken)
        {
            var response = await apiInvoker.ExecuteAsync((token, ct) => adminShipmentsQuery.GetAdminShipmentByIdAsync(shipmentId, token!, ct), cancellationToken: cancellationToken);

            return response.IsSuccess ? View(response.Data) : HandleGetFailure(response);
        }
    }
}