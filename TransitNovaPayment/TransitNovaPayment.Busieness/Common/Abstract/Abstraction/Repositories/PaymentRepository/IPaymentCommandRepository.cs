using TransitNovaPayment.Busieness.Common.DTO.PaymentDto;
using TransitNovaPayment.Busieness.Models.PaymentEntity;
namespace TransitNovaPayment.Busieness.Common.Abstract.Abstraction.Repositories.PaymentRepository
{
    public interface IPaymentCommandRepository
    {
        Task CreatePaymentAsync(Payment payment, CancellationToken cancellationToken);
    }
    public interface ICommandQueryRepository
    {




    }
}
