using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using TransitNova.BusinessLayer.DTOs.ZoneDtos;
using TransitNova.BusinessLayer.Interfaces.Repositories.LocationRepository;
using TransitNova.Domain.Entities.MainEntities;
using TransitNova.InfraStructure.Context;
using TransitNova.InfraStructure.Repository.Generic;
namespace TransitNova.InfraStructure.Repository.Location
{
    public class ZoneRepository(AppDbContext context, IMapper mapper) 
        : GenericRepository< Zone, Guid>(context, mapper.ConfigurationProvider)
        ,IZoneRepository
    {
        public async Task<Zone?> GetEntityByIdAsync(Guid id, CancellationToken ct)
            => await context.Zones.FirstOrDefaultAsync(z => z.Id == id, ct);
        public async Task<(List<ZoneDto> Items, int TotalCount)> FilterAsync(ZoneFilterDto filter, CancellationToken ct)
        {
            var query = context.Zones.AsNoTracking().AsQueryable();

            if (filter.CityId.HasValue)
                query = query.Where(z => z.CityId == filter.CityId.Value);

            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                var term = filter.SearchTerm.Trim().ToLower();
                query = query.Where(z =>
                    EF.Functions.Like(z.Name.ToLower(), $"%{term}%") ||
                    EF.Functions.Like(z.Code.ToLower(), $"%{term}%"));
            }

            query = filter.SortDescending ? query.OrderByDescending(z => z.Name) : query.OrderBy(z => z.Name);

            var total = await query.CountAsync(ct);
            var items = await query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ProjectTo<ZoneDto>(mapper.ConfigurationProvider)
                .ToListAsync(ct);

            return (items, total);
        }
        public async Task<(List<ZoneDto> Items, int TotalCount)> GetByCityIdAsync(int cityId, ZoneFilterDto filter, CancellationToken ct)
        {
            var f = new ZoneFilterDto
            {
                CityId = cityId,
                SearchTerm = filter.SearchTerm,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                SortDescending = filter.SortDescending
            };

            return await FilterAsync(f, ct);
        }
        public async Task<bool> NameExistsInCityAsync(int cityId, string name, CancellationToken ct)
        {
            var n = name.Trim().ToLower();
            return await context.Zones.AsNoTracking().AnyAsync(z => z.CityId == cityId && z.Name.ToLower() == n, ct);
        }
        public async Task<bool> CodeExistsInCityAsync(int cityId, string code, CancellationToken ct)
        {
            var c = code.Trim().ToLower();
            return await context.Zones.AsNoTracking().AnyAsync(z => z.CityId == cityId && z.Code.ToLower() == c, ct);
        }
        public async Task<bool> NameExistsInCityForAnotherAsync(Guid zoneId, int cityId, string name, CancellationToken ct)
        {
            var n = name.Trim().ToLower();
            return await context.Zones.AsNoTracking().AnyAsync(z => z.Id != zoneId && z.CityId == cityId && z.Name.ToLower() == n, ct);
        }
        public async Task<bool> CodeExistsInCityForAnotherAsync(Guid zoneId, int cityId, string code, CancellationToken ct)
        {
            var c = code.Trim().ToLower();
            return await context.Zones.AsNoTracking().AnyAsync(z => z.Id != zoneId && z.CityId == cityId && z.Code.ToLower() == c, ct);
        }
        public async Task<bool> CityExistsAsync(int cityId, CancellationToken ct)
            => await context.Cities.AsNoTracking().AnyAsync(c => c.Id == cityId, ct);
    }
}

