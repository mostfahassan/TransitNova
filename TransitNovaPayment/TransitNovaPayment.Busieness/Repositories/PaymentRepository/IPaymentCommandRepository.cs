using TransitNovaPayment.Busieness.Models.PaymentEntity;
namespace TransitNovaPayment.Busieness.Repositories.PaymentRepository
{
    public interface IPaymentCommandRepository
    {
        Task CreatePaymentAsync(Payment payment, CancellationToken cancellationToken);
    }
    
}
