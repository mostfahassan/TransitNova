using Microsoft.EntityFrameworkCore;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Payment;
using TransitNova.BusinessLayer.Interfaces.Repositories.PaymentInvoiceRepository;
using TransitNova.Domain.Entities.MainEntities;
using TransitNova.InfraStructure.Context;
namespace TransitNova.InfraStructure.Repository.PaymmentInvoice
{
    internal sealed class PaymentRepositoryQuery(AppDbContext context) : IPaymentRepositoryQuery
    {
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
                .Where(invoice => invoice.CustomerId == userId)
                .OrderByDescending(invoice => invoice.CreatedAt);

            return await ProjectInvoices(query).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<ShipmentPaymentInvoiceDto?> GetUserInvoiceByPaymentIdAsync(Guid userId, Guid paymentId, CancellationToken cancellationToken)
        {
            var query = context.PaymentInvoices
                .AsNoTracking()
                .Where(invoice => invoice.CustomerId == userId && invoice.PaymentId == paymentId)
                .OrderByDescending(invoice => invoice.CreatedAt);

            return await ProjectInvoices(query).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<IEnumerable<ShipmentPaymentInvoiceDto>> GetUserInvoicesAsync(Guid userId, CancellationToken cancellationToken)
        {
            var query = context.PaymentInvoices
                .AsNoTracking()
                .Where(invoice => invoice.CustomerId == userId)
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
                       on invoice.ShipmentId equals shipment.Id
                   select new ShipmentPaymentInvoiceDto
                   {
                       InvoiceId = "INV-" + invoice.Id.ToString().Substring(0, 8),
                       PaymentId = invoice.PaymentId,
                       ReferenceId = invoice.ShipmentId,
                       ReferenceType = "Shipment",
                       ShipmentId = invoice.ShipmentId,
                       ShipmentTrackingNumber = shipment.TrackingNumber,
                       CustomerName = invoice.UserProfile != null ? $"{invoice.UserProfile.FullName}" : string.Empty,
                       ShippingCost = invoice.ShippingCost,
                       Commission = invoice.Commission,
                       TotalAmount = invoice.Amount,
                       PaymentMethod = invoice.PaymentMethod.ToString(),
                       Status = invoice.Status.ToString(),
                       PaidAt = invoice.PaidAt,
                       Currency = shipment.Currency,
                       Notes = invoice.Notes
                   };
        }
    }
}
