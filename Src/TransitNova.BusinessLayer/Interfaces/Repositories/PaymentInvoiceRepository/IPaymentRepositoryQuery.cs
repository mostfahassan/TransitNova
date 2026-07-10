using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Payment;

namespace TransitNova.BusinessLayer.Interfaces.Repositories.PaymentInvoiceRepository
{
    public interface IPaymentRepositoryQuery
    {
        Task<PaymentInvoiceDto?> GetInvoiceByPaymentIdAsync(Guid paymentId, CancellationToken cancellationToken);
        Task<PaymentInvoiceDto?> GetUserInvoiceAsync(Guid userId, CancellationToken cancellationToken);
        Task<PaymentInvoiceDto?> GetUserInvoiceByPaymentIdAsync(Guid userId, Guid paymentId, CancellationToken cancellationToken);
        Task<IEnumerable<PaymentInvoiceDto>> GetUserInvoicesAsync(Guid userId, CancellationToken cancellationToken);
        Task<PagedResult<PaymentInvoiceDto>> GetInvoicesPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);
    }
}
