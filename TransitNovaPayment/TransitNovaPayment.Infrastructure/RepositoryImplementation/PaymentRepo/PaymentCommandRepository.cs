using TransitNovaPayment.Busieness.Models.PaymentEntity;
using TransitNovaPayment.Busieness.Repositories.PaymentRepository;
using TransitNovaPayment.Infrastructure.Context;

namespace TransitNovaPayment.Infrastructure.RepositoryImplementation.PaymentRepo
{
    internal class PaymentCommandRepository(AppDbContext context) : IPaymentCommandRepository
    {
        public async Task CreatePaymentAsync(Payment payment, CancellationToken cancellationToken)
        {
            await context.Payments.AddAsync(payment, cancellationToken);
        }
    }
}
