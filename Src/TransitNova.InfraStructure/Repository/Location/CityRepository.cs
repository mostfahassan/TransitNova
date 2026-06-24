using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using TransitNova.BusinessLayer.DTOs.City;
using TransitNova.BusinessLayer.Interfaces.Repositories.LocationRepository;
using TransitNova.Domain.Entities.MainEntities;
using TransitNova.InfraStructure.Context;
using TransitNova.InfraStructure.Repository.Generic;
namespace TransitNova.InfraStructure.Repository.Location
{
    public class CityRepository(AppDbContext context, IMapper mapper)
        : GenericRepository<City, int>(context, mapper.ConfigurationProvider)
        , ICityRepository
    {
      
        public async Task<(List<CityDto> Items, int TotalCount)> FilterAsync(CityFilterDto filter, CancellationToken ct)
        {
            var query = context.Cities.AsNoTracking().AsQueryable();

            if (filter.GovernmentId.HasValue)
                query = query.Where(c => c.GovernmentId == filter.GovernmentId.Value);

            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                var term = filter.SearchTerm.Trim().ToLower();
                query = query.Where(c => EF.Functions.Like(c.Name.ToLower(), $"%{term}%"));
            }

            query = filter.SortDescending ? query.OrderByDescending(c => c.Name) : query.OrderBy(c => c.Name);

            var total = await query.CountAsync(ct);
            var items = await query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ProjectTo<CityDto>(mapper.ConfigurationProvider)
                .ToListAsync(ct);

            return (items, total);
        }
        public async Task<IEnumerable<CityDto>> GetAllAsync(CancellationToken ct, int? id = null)
        {
            var cities = await context.Cities.AsNoTracking()
                .Where(c => c.GovernmentId == id)
                .Select(c => new CityDto
                {
                    Id = c.Id,
                    Name = c.Name,
                })
                .ToListAsync(ct);
            return cities;
        }
        public async Task<bool> NameExistsAsync(string name, CancellationToken ct, int? id = null)
        {
            var n = name.Trim().ToLower();
            if (id.HasValue)
            {
                return await context.Cities.AsNoTracking().AnyAsync(c => c.Id != id.Value && c.Name.ToLower() == n, ct);
            }
            return await context.Cities.AsNoTracking().AnyAsync(c =>c.Name.ToLower() == n, ct);
        }
        public async Task<bool> NameExistsForAnotherAsync(int id, string name , CancellationToken ct, int? anotherId = null)
        {
            var n = name.Trim().ToLower();
            if (anotherId.HasValue)
            {
                return await context.Cities.AsNoTracking().AnyAsync(c => c.Id != id && c.GovernmentId == anotherId.Value && c.Name.ToLower() == n, ct);
            }
            return await context.Cities.AsNoTracking().AnyAsync(c => c.Id != id && c.Name.ToLower() == n, ct);
        }
        public async Task<bool> NameExistsForAnotherGovernmentAsync(int governmentId, string name, CancellationToken ct)
        {
            var n = name.Trim().ToLower();
            return await context.Cities.AsNoTracking().AnyAsync(c => c.GovernmentId == governmentId && c.Name.ToLower() == n, ct);
        }


    }
}

