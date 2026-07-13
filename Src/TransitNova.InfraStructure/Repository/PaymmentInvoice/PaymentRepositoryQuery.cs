using Microsoft.EntityFrameworkCore;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Payment;
using TransitNova.BusinessLayer.Interfaces.Repositories.PaymentInvoiceRepository;
using TransitNova.Domain.Contracts.Constants;
using TransitNova.Domain.Entities.MainEntities;
using TransitNova.Domain.Enums.Shipment;
using TransitNova.InfraStructure.Context;
namespace TransitNova.InfraStructure.Repository.PaymmentInvoice
{
    internal sealed class PaymentRepositoryQuery(AppDbContext context) : IPaymentRepositoryQuery
    {
        public async Task<BundlePaymentInvoiceDto?> GetBundleInvoiceByPaymentIdAsync(Guid paymentId, CancellationToken cancellationToken)
        {
            var query =
                from invoice in context.PaymentInvoices.AsNoTracking()
                where invoice.PaymentId == paymentId
                    && invoice.ReferecneType == Constant.PaymentReferenceConstants.Bundle
                join bundle in context.Bundles.AsNoTracking()
                    on invoice.ReferecneId equals bundle.Id
                join user in context.UserProfiles.AsNoTracking()
                    on invoice.CustomerId equals user.Id
                let subscription = context.UserBundleSubscriptions.AsNoTracking()
                    .Where(item => item.BundleId == bundle.Id && item.SubscribedUserId == invoice.CustomerId)
                    .OrderByDescending(item => item.SubscriptionDate)
                    .Select(item => new
                    {
                        item.SubscriptionDate,
                        item.EndDate
                    })
                    .FirstOrDefault()
                orderby invoice.CreatedAt descending
                select new BundlePaymentInvoiceDto
                {
                    InvoiceId = "INV-" + invoice.Id.ToString().Substring(0, 8),
                    PaymentId = invoice.PaymentId,
                    ReferenceId = invoice.ReferecneId,
                    ReferenceType = Constant.PaymentReferenceConstants.Bundle,
                    BundleId = bundle.Id,
                    BundleName = bundle.BundleName,
                    FullName = user.FullName,
                    BundlePrice = invoice.Cost > 0 ? invoice.Cost : bundle.BundlePrice,
                    Commission = invoice.Commission,
                    TotalAmount = invoice.Amount,
                    PaymentMethod = invoice.PaymentMethod.ToString(),
                    Status = invoice.Status.ToString(),
                    Currency = Currency.EGP,
                    PaidAt = invoice.PaidAt,
                    EndDate = subscription != null ? subscription.EndDate : null,
                    SubscribedAt = subscription != null ? subscription.SubscriptionDate : null,
                    Notes = invoice.Notes
                };

            return await query.FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<ShipmentPaymentInvoiceDto?> GetInvoiceByPaymentIdAsync(Guid paymentId, CancellationToken cancellationToken)
        {
            var query = context.PaymentInvoices
                .AsNoTracking()
                .Where(invoice => invoice.PaymentId == paymentId)
                .OrderByDescending(invoice => invoice.CreatedAt);

            return await ProjectInvoices(query).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<ShipmentPaymentInvoiceDto?> GetUserInvoiceAsync(Guid userId, CancellationToken cancellationToken)
        {
            var query = context.PaymentInvoices
                .AsNoTracking()
                .Where(invoice => invoice.CustomerId == userId || invoice.UserProfile.AppUserId == userId)
                .OrderByDescending(invoice => invoice.CreatedAt);

            return await ProjectInvoices(query).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<ShipmentPaymentInvoiceDto?> GetUserInvoiceByPaymentIdAsync(Guid userId, Guid paymentId, CancellationToken cancellationToken)
        {
            var query = context.PaymentInvoices
                .AsNoTracking()
                .Where(invoice => (invoice.CustomerId == userId || invoice.UserProfile.AppUserId == userId) && invoice.PaymentId == paymentId)
                .OrderByDescending(invoice => invoice.CreatedAt);

            return await ProjectInvoices(query).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<IEnumerable<ShipmentPaymentInvoiceDto>> GetUserInvoicesAsync(Guid userId, CancellationToken cancellationToken)
        {
            var query = context.PaymentInvoices
                .AsNoTracking()
                .Where(invoice => invoice.CustomerId == userId || invoice.UserProfile.AppUserId == userId)
                .OrderByDescending(invoice => invoice.CreatedAt);

            return await ProjectInvoices(query).ToListAsync(cancellationToken);
        }

        public async Task<PagedResult<ShipmentPaymentInvoiceDto>> GetInvoicesPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            pageNumber = Math.Max(1, pageNumber);
            pageSize = Math.Clamp(pageSize, 1, 100);

            var query = context.PaymentInvoices.AsNoTracking();
            var totalCount = await query.CountAsync(cancellationToken);

            var pageQuery = query
                .OrderByDescending(invoice => invoice.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            var invoices = await ProjectInvoices(pageQuery).ToListAsync(cancellationToken);
            return PagedResult<ShipmentPaymentInvoiceDto>.From(invoices, totalCount, pageNumber, pageSize);
        }

        private IQueryable<ShipmentPaymentInvoiceDto> ProjectInvoices(IQueryable<PaymentInvoice> invoices)
        {
            return from invoice in invoices
                   join shipment in context.Shipments.AsNoTracking()
                       on invoice.ReferecneId equals shipment.Id
                   select new ShipmentPaymentInvoiceDto
                   {
                       InvoiceId = "INV-" + invoice.Id.ToString().Substring(0, 8),
                       PaymentId = invoice.PaymentId,
                       ReferenceId = invoice.ReferecneId,
                       ReferenceType = Constant.PaymentReferenceConstants.Shipment,
                       ShipmentId = invoice.ReferecneId,
                       ShipmentTrackingNumber = shipment.TrackingNumber,
                       CustomerName = invoice.UserProfile != null ? $"{invoice.UserProfile.FullName}" : string.Empty,
                       ShippingCost = invoice.FinalShippingCost > 0 ? invoice.FinalShippingCost : invoice.Cost,
                       Commission = invoice.Commission,
                       TotalAmount = invoice.Amount,
                       PaymentMethod = invoice.PaymentMethod.ToString(),
                       Status = invoice.Status.ToString(),
                       PaidAt = invoice.PaidAt,
                       Currency = shipment.Currency,
                       Notes = invoice.Notes,
                       OriginalShippingCost = invoice.OriginalShippingCost > 0 ? invoice.OriginalShippingCost : invoice.Cost,
                       DiscountAmount = invoice.DiscountAmount,
                       DiscountPercentage = invoice.DiscountPercentage,
                       FinalShippingCost = invoice.FinalShippingCost > 0 ? invoice.FinalShippingCost : invoice.Cost,
                       BundleName = invoice.BundleName,
                       SubscriptionBenefitApplied = invoice.SubscriptionBenefitApplied,
                       SubscriptionBenefitMessage = invoice.SubscriptionBenefitApplied
                            ? "Subscription benefit applied to this shipment."
                            : string.Empty
                   };
        }
    }
}
