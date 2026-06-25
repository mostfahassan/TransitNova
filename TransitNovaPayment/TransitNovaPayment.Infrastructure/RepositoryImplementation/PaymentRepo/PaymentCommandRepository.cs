using TransitNovaPayment.Busieness.Common.Abstract.Abstraction.Interfaces;
using TransitNovaPayment.Busieness.Common.Abstract.Abstraction.Repositories.PaymentRepository;
using TransitNovaPayment.Busieness.Contracts.Keys;
using TransitNovaPayment.Busieness.Models.PaymentEntity;
using TransitNovaPayment.Infrastructure.Context;
namespace TransitNovaPayment.Infrastructure.RepositoryImplementation.PaymentRepo
{
    internal class PaymentCommandRepository(AppDbContext context, ICacheService cacheService) : IPaymentCommandRepository
    {
        public async Task CreatePaymentAsync(Payment payment, CancellationToken cancellationToken)
        {
            await context.Payments.AddAsync(payment, cancellationToken);
            await cacheService.RemoveByPrefixAsync(CacheKeys.PaymentsPrefix, cancellationToken);
        }
    }
}