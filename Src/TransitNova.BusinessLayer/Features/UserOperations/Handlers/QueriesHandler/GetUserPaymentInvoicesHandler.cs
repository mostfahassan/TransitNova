using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Payment;
using TransitNova.BusinessLayer.Features.UserOperations.Queries;
using TransitNova.BusinessLayer.Interfaces.Repositories.PaymentInvoiceRepository;
namespace TransitNova.BusinessLayer.Features.UserOperations.Handlers.QueriesHandler
{
    public sealed class GetUserPaymentInvoiceHandler(
        IPaymentRepositoryQuery paymentRepositoryQuery,
        ILogger<GetUserPaymentInvoiceHandler> logger)
        : IQueryHandler<GetUserPaymentInvoiceQuery, Result<PaymentInvoiceDto>>
    {
        public async Task<Result<PaymentInvoiceDto>> Handle(GetUserPaymentInvoiceQuery request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Retrieving payment invoice {PaymentId} for User {UserId}", request.PaymentId, request.AppUserId);

            var invoice = await paymentRepositoryQuery.GetUserInvoiceByPaymentIdAsync(request.AppUserId, request.PaymentId, cancellationToken);
            if (invoice is null)
            {
                logger.LogWarning("Payment invoice {PaymentId} was not found for User {UserId}", request.PaymentId, request.AppUserId);
                return Result<PaymentInvoiceDto>.NotFound(Errors.NotFound("Payment invoice was not found."));
            }

            return Result<PaymentInvoiceDto>.Success(invoice);
        }
    }

    public sealed class GetUserPaymentInvoicesHandler(
        IPaymentRepositoryQuery paymentRepositoryQuery,
        ILogger<GetUserPaymentInvoicesHandler> logger)
        : IQueryHandler<GetUserPaymentInvoicesQuery, Result<IEnumerable<PaymentInvoiceDto>>>
    {
        public async Task<Result<IEnumerable<PaymentInvoiceDto>>> Handle(GetUserPaymentInvoicesQuery request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Retrieving payment invoices for User {UserId}", request.AppUserId);

            var invoices = await paymentRepositoryQuery.GetUserInvoicesAsync(request.AppUserId, cancellationToken);
            return Result<IEnumerable<PaymentInvoiceDto>>.Success(invoices);
        }
    }
}
