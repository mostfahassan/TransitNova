using TransitNovaUI.BusinessLayer.DTOs.Payment;
using TransitNovaUI.BusinessLayer.DTOs.Shipment;

namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.User.Shipments.Commands;

public interface ICreateShipmentCommandService
{
    Task<ApiResponse<UiInvoiceDto>> CreateShipmentAsync(UiCreateShipmentDto model, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default);
}