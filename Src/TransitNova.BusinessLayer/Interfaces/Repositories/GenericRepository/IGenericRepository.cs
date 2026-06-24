
using System.Linq.Expressions;
using TransitNova.Domain.Entities.Common;

namespace TransitNova.BusinessLayer.Interfaces.Repositories.GenericRepository
{
    public interface IGenericRepository<TEntity, TKey>
     where TEntity : BaseEntity<TKey>
    {
        Task<List<TRetrieve>> GetListAsync<TRetrieve>(CancellationToken ct);
        Task<TRetrieve?> GetByIdAsync<TRetrieve>(TKey id, CancellationToken ct);
        Task<TRetrieve?> FindAsync<TRetrieve>(Expression<Func<TEntity, bool>> predicate, CancellationToken ct);
        Task<bool> ExistsAsync(TKey id, CancellationToken ct);
        Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken ct = default);
        Task AddAsync(TEntity entity, CancellationToken ct);
        void Update(TEntity entity);
        Task<bool> DeleteAsync(TKey id, CancellationToken ct);
    }
}
