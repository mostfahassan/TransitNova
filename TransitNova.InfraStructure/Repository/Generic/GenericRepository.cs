
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TransitNova.BusinessLayer.Interfaces.Repositories.GenericRepository;
using TransitNova.Domain.Entities.Common;
using TransitNova.InfraStructure.Context;
namespace TransitNova.InfraStructure.Repository.Generic
{
    public class GenericRepository<TEntity, TKey>(AppDbContext context, IConfigurationProvider mapperConfig)
     : IGenericRepository<TEntity, TKey>
     where TEntity : BaseEntity<TKey>
    {
        private readonly DbSet<TEntity> _dbSet = context.Set<TEntity>();

        public async Task<List<TRetrieve>> GetListAsync<TRetrieve>(CancellationToken ct)
            => await _dbSet
                .AsNoTracking()
                .ProjectTo<TRetrieve>(mapperConfig)
                .ToListAsync(ct);

        public async Task<TRetrieve?> GetByIdAsync<TRetrieve>(TKey id, CancellationToken ct)
            => await _dbSet
                .AsNoTracking()
                .Where(x => x.Id!.Equals(id))
                .ProjectTo<TRetrieve>(mapperConfig)
                .FirstOrDefaultAsync(ct);

        public async Task<TRetrieve?> FindAsync<TRetrieve>(Expression<Func<TEntity, bool>> predicate, CancellationToken ct)
            => await _dbSet
                .AsNoTracking()
                .Where(predicate)
                .ProjectTo<TRetrieve>(mapperConfig)
                .FirstOrDefaultAsync(ct);

        public async Task<bool> ExistsAsync(TKey id, CancellationToken ct)
            => await _dbSet.AnyAsync(x => x.Id!.Equals(id), ct);

        public async Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken ct = default)
            => predicate is null
                ? await _dbSet.CountAsync(ct)
                : await _dbSet.CountAsync(predicate, ct);

        public async Task AddAsync(TEntity entity, CancellationToken ct)
            => await _dbSet.AddAsync(entity, ct);

        public void Update(TEntity entity)
            => _dbSet.Update(entity);

        public async Task<bool> DeleteAsync(TKey id, CancellationToken ct)
        {
            var entity = await _dbSet.FindAsync([id], ct);
            if (entity is null) return false;

            _dbSet.Remove(entity);
            return true;
        }
    }
}
