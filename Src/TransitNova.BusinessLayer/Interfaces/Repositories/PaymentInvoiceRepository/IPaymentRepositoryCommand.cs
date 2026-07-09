using TransitNova.Domain.Entities.MainEntities;

namespace TransitNova.BusinessLayer.Interfaces.Repositories.PaymentInvoiceRepository
{
    public interface IPaymentRepositoryCommand
    {
        Task CreateInvoice(PaymentInvoice invoice, CancellationToken cancellationToken);
    }

   
}
