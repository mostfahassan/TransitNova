namespace TransitNovaPayment.Busieness.Interfaces.Common
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync(CancellationToken ct);
        Task BeginTransactionAsync(CancellationToken ct);
        Task CommitAsync(CancellationToken ct);
        Task RollbackAsync(CancellationToken ct);
    }
}
