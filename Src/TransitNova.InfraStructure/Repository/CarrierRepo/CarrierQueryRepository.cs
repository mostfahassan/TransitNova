using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Carrier;
using TransitNova.BusinessLayer.Interfaces.Repositories.CarrierRepository;
using TransitNova.Domain.Entities.MainEntities;
using TransitNova.Domain.Enums.Carrier;
using TransitNova.InfraStructure.Context;

namespace TransitNova.InfraStructure.Repository.CarrierRepo
{
    internal class CarrierQueryRepository(ILogger<CarrierQueryRepository> logger , IMapper mapper, AppDbContext context) : ICarrierQueryRepository
    {
        public async Task<PagedResult<TRetrieve>> FilterByCriteriaAsync<TRetrieve>(FilterCarrierDto filterCriteria, bool tracked = false, CancellationToken ct = default)
        {
            var query = tracked
                ? context.Carriers.AsQueryable()
                : context.Carriers.AsNoTracking();

            if (filterCriteria.Status.HasValue)
                query = query.Where(c => c.Status == filterCriteria.Status.Value);

            if (filterCriteria.MinRating.HasValue)
                query = query.Where(c => c.AverageRating >= filterCriteria.MinRating.Value);

            if (filterCriteria.MaxRating.HasValue)
                query = query.Where(c => c.AverageRating <= filterCriteria.MaxRating.Value);

            if (filterCriteria.WarehouseId.HasValue)
                query = query.Where(c => c.HomeWarehouseId == filterCriteria.WarehouseId.Value);

            if (filterCriteria.MinYearsOfExperience.HasValue)

                query = query.Where(c => c.YearsOfExperience >= filterCriteria.MinYearsOfExperience.Value);
           
            if (filterCriteria.CityId.HasValue)
                query = query.Where(c => c.ServedZones.Any(z => z.CityId == filterCriteria.CityId.Value));

            if (filterCriteria.MaxYearsOfExperience.HasValue)
                query = query.Where(c => c.YearsOfExperience <= filterCriteria.MaxYearsOfExperience.Value);


            if (!string.IsNullOrWhiteSpace(filterCriteria.City))
            {
                var city = filterCriteria.City.Trim().ToLower();

                query = query.Where(c =>
                    EF.Functions.Like(c.City.Name.ToLower(), $"%{city}%"));
            }

            if (!string.IsNullOrWhiteSpace(filterCriteria.SearchTerm))
            {
                var term = filterCriteria.SearchTerm.Trim().ToLower();

                query = query.Where(c =>
                    EF.Functions.Like(c.FullName.ToLower(), $"%{term}%") ||
                    EF.Functions.Like(c.LicenseNumber.ToLower(), $"%{term}%"));
            }

            if (filterCriteria.AvailableFrom.HasValue)
                query = query.Where(c => c.Status == CarrierStatus.Available);

            if (filterCriteria.VehicleType.HasValue)
                query = query.Where(c =>
                    c.Vehicle != null &&
                    c.Vehicle.VehicleType == filterCriteria.VehicleType.Value);

            if (filterCriteria.VehicleCapacityWeight.HasValue)
                query = query.Where(c =>
                    c.Vehicle != null &&
                    c.Vehicle.CapacityWeight >= filterCriteria.VehicleCapacityWeight.Value);

            if (filterCriteria.ServedZones is { Count: > 0 })
            {
                var zones = filterCriteria.ServedZones
                    .Where(z => !string.IsNullOrWhiteSpace(z))
                    .Select(z => z.Trim().ToLower())
                    .Distinct()
                    .ToList();

                if (zones.Count > 0)
                {
                    query = query.Where(c =>
                        c.ServedZones.Any(z =>
                            zones.Contains(z.Name.ToLower())));
                }
            }

            var totalCount = await query.CountAsync(ct);

            if (totalCount == 0)
            {
                return PagedResult<TRetrieve>.From(
                    [],
                    totalCount,
                    filterCriteria.PageNumber,
                    filterCriteria.PageSize);
            }

            query = filterCriteria.SortBy switch
            {
                CarrierSortBy.Rating => filterCriteria.SortDescending
                    ? query.OrderByDescending(c => c.AverageRating)
                    : query.OrderBy(c => c.AverageRating),

                CarrierSortBy.Yearsofexperience => filterCriteria.SortDescending
                    ? query.OrderByDescending(c => c.YearsOfExperience)
                    : query.OrderBy(c => c.YearsOfExperience),

                CarrierSortBy.Name => filterCriteria.SortDescending
                    ? query.OrderByDescending(c => c.LastName)
                    : query.OrderBy(c => c.FirstName),

                CarrierSortBy.Status => filterCriteria.SortDescending
                    ? query.OrderByDescending(c => c.Status)
                    : query.OrderBy(c => c.Status),

                _ => query.OrderByDescending(c => c.AverageRating)
            };

            var items = await query
                .Skip((filterCriteria.PageNumber - 1) * filterCriteria.PageSize)
                .Take(filterCriteria.PageSize)
                .ProjectTo<TRetrieve>(mapper.ConfigurationProvider)
                .ToListAsync(ct);

           return PagedResult<TRetrieve>.From(items, totalCount, filterCriteria.PageNumber, filterCriteria.PageSize);

        }

        public async Task<Carrier?> GetCarrierAsync(Expression<Func<Carrier, bool>> predicate, CancellationToken cancellationToken )
             => await context.Carriers.Where(predicate).FirstOrDefaultAsync(cancellationToken);
           
        

        public async Task<Carrier?> GetCarrierByAppUserIdAsync(Guid appUserId, bool tracked = false, CancellationToken ct = default)
        {
            logger.LogTrace("Fetching Carrier by User Id: {UserId}", appUserId);
            var carrierQuery = context.Carriers.AsQueryable();
            if (!tracked)

                carrierQuery = carrierQuery.AsNoTracking();
                               
            var carrier = await carrierQuery.FirstOrDefaultAsync(c => c.AppUserId == appUserId, ct);
               
            if (carrier == null)
                logger.LogWarning("Carrier for User Id {UserId} not found", appUserId);

            return carrier;
            
        }

        public async Task<TRetrieve?> GetCarrierDetailsAsync<TRetrieve>(Guid carrierId,CancellationToken ct = default)
        {
            logger.LogTrace("Fetching Carrier info for Id: {UserId}", carrierId);
            var carrierQuery = context.Carriers.AsQueryable().AsNoTracking();
            var carrier = await carrierQuery
                .Where(c => c.Id == carrierId)
                .ProjectTo<TRetrieve>(mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(ct);

            if (carrier == null)
                logger.LogWarning("Detailed Carrier info for Id {UserId} not found", carrierId);

            return carrier;
        }

      

        public async Task<IEnumerable<CarrierProfileDto>> GetCarriersInStatusAsync(CarrierStatus status, CancellationToken ct = default)
        {
            var carrierInStatus = await context.Carriers
               .AsNoTracking()
               .Where(c => c.Status == status)
               .ProjectTo<CarrierProfileDto>(mapper.ConfigurationProvider)
               .ToListAsync(ct);
            return carrierInStatus;
        }

        public async Task<string?> GetCarrierNameAsync(Guid carrierId, CancellationToken ct = default)
            => await context.Carriers
                .AsNoTracking()
                .Where(carrier => carrier.Id == carrierId)
                .Select(carrier => carrier.FullName)
                .FirstOrDefaultAsync(ct);

        public async Task<CarrierStatus> GetStatusAsync(Guid carrierId, CancellationToken ct = default)
        {
            logger.LogTrace("Fetching status for Carrier {UserId}", carrierId);

            var status = await context.Carriers
                .AsNoTracking()
                .Where(c => c.Id == carrierId)
                .Select(c => c.Status)
                .FirstOrDefaultAsync(ct);
          
            return status;
        }

        public async Task<Carrier?> GetCarrierForTripAsync(Expression<Func<Carrier, bool>> predicate, CancellationToken cancellationToken = default)
          => await context.Carriers.Where(predicate).Include(c =>c.Trips).FirstOrDefaultAsync(cancellationToken);
    }
}
