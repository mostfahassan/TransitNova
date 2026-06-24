using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using TransitNova.BusinessLayer.DTOs.Country;
using TransitNova.BusinessLayer.Interfaces.Repositories.LocationRepository;
using TransitNova.Domain.Entities.MainEntities;
using TransitNova.InfraStructure.Context;
using TransitNova.InfraStructure.Repository.Generic;

namespace TransitNova.InfraStructure.Repository.Location
{
    public class CountryRepository(AppDbContext context, IMapper mapper) : 
        GenericRepository<Country, int>(context, mapper.ConfigurationProvider),
        ICountryRepository
    {
        public async Task<(List<CountryDto> Items, int TotalCount)> FilterAsync(CountryFilterDto filter, CancellationToken ct)
        {
            var query = context.Countries.AsNoTracking().AsQueryable();

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
                .ProjectTo<CountryDto>(mapper.ConfigurationProvider)
                .ToListAsync(ct);

            return (items, total);
        }
        
        public async Task<bool> NameExistsForAnotherAsync(int countryId, string name, CancellationToken ct , int? anotherId = null)
        {
            
            var n = name.Trim().ToLower();
            return await context.Countries.AsNoTracking().AnyAsync(c => c.Id != countryId && c.Name.ToLower() == n, ct);
        }
        public async Task<IEnumerable<GovernmentDto>> GetAllGovernmentsAsync(int countryId, CancellationToken ct)
        {
            return await context.Governments
                .AsNoTracking()
                .Where(c => c.CountryId == countryId)
                .OrderBy(c => c.Name)
                .ProjectTo<GovernmentDto>(mapper.ConfigurationProvider)
                .ToListAsync(ct);
        }
        public async Task<IEnumerable<CountryDto>> GetAllAsync(CancellationToken ct, int? id = null)
        {
            return await context.Countries
               .AsNoTracking()
               .OrderBy(c => c.Name)
               .ProjectTo<CountryDto>(mapper.ConfigurationProvider)
               .ToListAsync(ct);
        }

        public async Task<bool> NameExistsAsync(string name, CancellationToken ct, int? id = null)
        {
            var n = name.Trim().ToLower();
            if (id.HasValue)
            {
                return await context.Countries.AsNoTracking().AnyAsync(c => c.Id != id.Value && c.Name.ToLower() == n, ct);
            }
            return await context.Countries.AsNoTracking().AnyAsync(c => c.Name.ToLower() == n, ct);
        }
    }
}

