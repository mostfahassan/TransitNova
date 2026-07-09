using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Payment;

namespace TransitNova.BusinessLayer.Features.UserOperations.Queries
{
    public sealed record GetUserPaymentInvoiceQuery(Guid AppUserId, Guid PaymentId) : IQuery<Result<PaymentInvoiceDto>>;

    public sealed record GetUserPaymentInvoicesQuery(Guid AppUserId) : IQuery<Result<IEnumerable<PaymentInvoiceDto>>>;
}
