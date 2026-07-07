namespace TransitNova.BusinessLayer.Interfaces.Services.UnitOfWork
{
    public interface IUnitOfWork:IDisposable

    {   
       Task<int> SaveChangesAsync(CancellationToken ct);
        Task BeginTransactionAsync(CancellationToken ct);
        Task CommitAsync(CancellationToken ct);
        Task RollbackAsync(CancellationToken ct);
        

    }
}
