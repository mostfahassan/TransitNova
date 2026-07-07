using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using TransitNova.BusinessLayer.Interfaces.Services.UnitOfWork;
using TransitNova.Domain;
using TransitNova.InfraStructure.Context;
namespace TransitNova.InfraStructure.Repository
{
    public class UnitOfWork(AppDbContext context) : IUnitOfWork, IDisposable, IAsyncDisposable
    {
        private IDbContextTransaction? _transaction;
        private bool _disposed;

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();

            var softDeletedEntries = context.ChangeTracker.Entries<ISoftDeletable>()
                 .Where(e => e.State == EntityState.Deleted);
            foreach (var softDeletedEntry in softDeletedEntries)
            {
                softDeletedEntry.State = EntityState.Modified;
                softDeletedEntry.Property(nameof(ISoftDeletable.IsDeleted)).CurrentValue = true;
                softDeletedEntry.Property(nameof(ISoftDeletable.DeletedOn)).CurrentValue = DateTime.UtcNow;
            }

            return await context.SaveChangesAsync(cancellationToken);
        }
        public async Task BeginTransactionAsync(CancellationToken ct)
        {
            ThrowIfDisposed();

            if (_transaction is not null)
                throw new InvalidOperationException("A database transaction is already active.");

            _transaction = await context.Database.BeginTransactionAsync(ct);
        }
        
        public async Task CommitAsync(CancellationToken ct)
        {
            ThrowIfDisposed();

            if (_transaction is null)
                return;

            try
            {
                await _transaction.CommitAsync(ct);
            }
            finally
            {
                await DisposeTransactionAsync();
            }
        }

        public async Task RollbackAsync(CancellationToken ct)
        {
            ThrowIfDisposed();

            if (_transaction is null)
                return;

            try
            {
                await _transaction.RollbackAsync(ct);
            }
            finally
            {
                await DisposeTransactionAsync();
            }
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _transaction?.Dispose();
            _transaction = null;
            _disposed = true;
            GC.SuppressFinalize(this);
        }

        public async ValueTask DisposeAsync()
        {
            if (_disposed)
                return;

            if (_transaction is not null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }

            _disposed = true;
            GC.SuppressFinalize(this);
        }

        private async ValueTask DisposeTransactionAsync()
        {
            if (_transaction is null)
                return;

            await _transaction.DisposeAsync();
            _transaction = null;
        }

        private void ThrowIfDisposed()
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
        }
    }
}
