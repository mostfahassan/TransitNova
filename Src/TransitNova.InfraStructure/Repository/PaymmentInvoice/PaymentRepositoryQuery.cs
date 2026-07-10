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
        public async Task<PaymentInvoiceDto?> GetInvoiceByPaymentIdAsync(Guid paymentId, CancellationToken cancellationToken)
        {
            var query = context.PaymentInvoices
                .AsNoTracking()
                .Where(invoice => invoice.PaymentId == paymentId)
                .OrderByDescending(invoice => invoice.CreatedAt);

            return await ProjectInvoices(query).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<PaymentInvoiceDto?> GetUserInvoiceAsync(Guid userId, CancellationToken cancellationToken)
        {
            var query = context.PaymentInvoices
                .AsNoTracking()
                .Where(invoice => invoice.CustomerId == userId)
                .OrderByDescending(invoice => invoice.CreatedAt);

            return await ProjectInvoices(query).FirstOrDefaultAsync(cancellationToken);
       
        }

        public async Task<PaymentInvoiceDto?> GetUserInvoiceByPaymentIdAsync(Guid userId, Guid paymentId, CancellationToken cancellationToken)
        {
            var query = context.PaymentInvoices
                .AsNoTracking()
                .Where(invoice => invoice.CustomerId == userId && invoice.PaymentId == paymentId)
                .OrderByDescending(invoice => invoice.CreatedAt);

            return await ProjectInvoices(query).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<IEnumerable<PaymentInvoiceDto>> GetUserInvoicesAsync(Guid userId, CancellationToken cancellationToken)
        {
            var query = context.PaymentInvoices
                .AsNoTracking()
                .Where(invoice => invoice.CustomerId == userId)
                .OrderByDescending(invoice => invoice.CreatedAt);

            return await ProjectInvoices(query).ToListAsync(cancellationToken);
        }

        public async Task<PagedResult<PaymentInvoiceDto>> GetInvoicesPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
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
            return PagedResult<PaymentInvoiceDto>.From(invoices, totalCount, pageNumber, pageSize);
        }

        private IQueryable<PaymentInvoiceDto> ProjectInvoices(IQueryable<PaymentInvoice> invoices)
        {
            return from invoice in invoices
                   join shipment in context.Shipments.AsNoTracking()
                       on invoice.ShipmentId equals shipment.Id
                   select new PaymentInvoiceDto
                   {
                       InvoiceId = "INV-" + invoice.Id.ToString().Substring(0, 8),
                       PaymentId = invoice.PaymentId,
                       ShipmentId = invoice.ShipmentId,
                       ShipmentTrackingNumber = shipment.TrackingNumber,
                       CustomerName = invoice.UserProfile != null ? $"{invoice.UserProfile.FullName}" : string.Empty,
                       ShippingCost = invoice.ShippingCost,
                       Commission = invoice.Commission,
                       TotalAmount = invoice.Amount,
                       PaymentMethod = invoice.PaymentMethod,
                       Status = invoice.Status,
                       PaidAt = invoice.PaidAt,
                       Currency = shipment.Currency,
                       Notes = invoice.Notes
                   };
        }
    }
}
