using Microsoft.EntityFrameworkCore.Storage;
using TransitNovaPayment.Busieness.Interfaces.Common;
using TransitNovaPayment.Infrastructure.Context;

namespace TransitNovaPayment.Infrastructure.RepositoryImplementation
{
    public class UnitOfWork(AppDbContext context) : IUnitOfWork
    {
        private IDbContextTransaction? _transaction;
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await context.SaveChangesAsync(cancellationToken);
        }
        public async Task BeginTransactionAsync(CancellationToken ct)
        {
            _transaction = await context.Database.BeginTransactionAsync(ct);
        }

        public async Task CommitAsync(CancellationToken ct)
        {
            if (_transaction != null)
                await _transaction.CommitAsync(ct);
        }

        public async Task RollbackAsync(CancellationToken ct)
        {
            if (_transaction != null)
                await _transaction.RollbackAsync(ct);
        }
    }
}
