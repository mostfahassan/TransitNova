using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using TransitNova.BusinessLayer.Interfaces;
using TransitNova.BusinessLayer.Interfaces.Services.UnitOfWork;
using TransitNova.InfraStructure.Context;

namespace TransitNova.InfraStructure.Repository
{
    public class UnitOfWork(AppDbContext context) : IUnitOfWork
    {
        private IDbContextTransaction? _transaction;
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
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
