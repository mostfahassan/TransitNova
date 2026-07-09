using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Payment;

namespace TransitNova.BusinessLayer.Interfaces.Repositories.PaymentInvoiceRepository
{
    public interface IPaymentRepositoryQuery
    {
        Task<PaymentInvoiceDto?> GetInvoiceByPaymentId(Guid paymentId, CancellationToken cancellationToken);
        Task<PaymentInvoiceDto?> GetUserInvoice(Guid userId, CancellationToken cancellationToken);
        Task<PaymentInvoiceDto?> GetUserInvoiceByPaymentId(Guid userId, Guid paymentId, CancellationToken cancellationToken);
        Task<IEnumerable<PaymentInvoiceDto>> GetUserInvoices(Guid userId, CancellationToken cancellationToken);
        Task<PagedResult<PaymentInvoiceDto>> GetInvoicesPaged(int pageNumber, int pageSize, CancellationToken cancellationToken);
    }
}
