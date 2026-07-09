using TransitNova.BusinessLayer.Interfaces.Repositories.PaymentInvoiceRepository;
using TransitNova.Domain.Entities.MainEntities;
using TransitNova.InfraStructure.Context;

namespace TransitNova.InfraStructure.Repository.PaymmentInvoice
{
    internal class PaymentRepositoryCommand(AppDbContext context) : IPaymentRepositoryCommand
    {
        public async Task CreateInvoice(PaymentInvoice invoice, CancellationToken cancellationToken)
          => await context.PaymentInvoices.AddAsync(invoice,cancellationToken);
    }
}
