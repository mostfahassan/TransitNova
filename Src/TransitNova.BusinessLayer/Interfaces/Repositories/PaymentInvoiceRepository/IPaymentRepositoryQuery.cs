using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Payment;

namespace TransitNova.BusinessLayer.Interfaces.Repositories.PaymentInvoiceRepository
{
    public interface IPaymentRepositoryQuery
    {
        Task<ShipmentPaymentInvoiceDto?> GetInvoiceByPaymentIdAsync(Guid paymentId, CancellationToken cancellationToken);
        Task<ShipmentPaymentInvoiceDto?> GetUserInvoiceAsync(Guid userId, CancellationToken cancellationToken);
        Task<ShipmentPaymentInvoiceDto?> GetUserInvoiceByPaymentIdAsync(Guid userId, Guid paymentId, CancellationToken cancellationToken);
        Task<IEnumerable<ShipmentPaymentInvoiceDto>> GetUserInvoicesAsync(Guid userId, CancellationToken cancellationToken);
        Task<PagedResult<ShipmentPaymentInvoiceDto>> GetInvoicesPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);
    }
}
