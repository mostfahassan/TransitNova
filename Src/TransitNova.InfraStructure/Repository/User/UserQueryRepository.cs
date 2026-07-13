
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using TransitNova.BusinessLayer.Common.CommonData;
using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Shipment;
using TransitNova.BusinessLayer.DTOs.UserProfile;
using TransitNova.BusinessLayer.Interfaces.Repositories.UserRepository;
using TransitNova.Domain.Enums.Shipment;
using TransitNova.InfraStructure.Context;

namespace TransitNova.InfraStructure.Repository.User
{
    public class UserQueryRepository(AppDbContext context,IMapper mapper) : IUserQueryRepository
    {
        public async Task<Guid> GetAppUserIdAsync(Guid AppUserId, CancellationToken ct)
             => await context.UserProfiles.AsNoTracking()
                .Where(u => u.AppUserId == AppUserId)
                .Select(u => u.Id)
                .FirstOrDefaultAsync(ct);

        public async Task<Guid?> GetAppUserIdByProfileIdAsync(Guid profileId, CancellationToken ct)
            => await context.UserProfiles.AsNoTracking()
                .Where(u => u.Id == profileId || u.AppUserId == profileId)
                .Select(u => (Guid?)u.AppUserId)
                .FirstOrDefaultAsync(ct);
           
     

        public async Task<IEnumerable<RetrieveShipmentSummaryDto>> GetUserShipmentsAsync(Guid AppUserId, CancellationToken ct)
           => await context.Shipments.AsNoTracking()
                .Where(sh => sh.Sender.AppUserId == AppUserId)
                .Select(sh => new RetrieveShipmentSummaryDto
                {
                    Id = sh.Id,
                    TrackingNumber = sh.TrackingNumber,
                    CurrentStatus = sh.CurrentStatus,
                    ShipmentType = sh.ShipmentType,
                    CreatedAt = sh.CreatedAt,
                    SenderCity = sh.Sender.City.Name,
                    ReceiverCity = sh.Receiver.City.Name,
                    Weight = sh.PackageSpecification.Weight
                })
                .ToListAsync(ct);
   

        public async Task<Dictionary<ShipmentStatuses, int>> GetShipmentCountInStatusAsync(Guid AppUserId, CancellationToken cancellationToken)
            => await context.Shipments
                                .AsNoTracking()
                                .Where(sh => sh.Sender.AppUserId == AppUserId)
                                .GroupBy(sh => sh.CurrentStatus)
                               .ToDictionaryAsync(
                                 g => g.Key,
                                 g => g.Count(),
                                 cancellationToken);

   
        public async Task<UserProfileDto?> GetUserProfileAsync(Guid userId, CancellationToken ct)
            => await context.UserProfiles
                .AsNoTracking()
                .Where(u => u.AppUserId == userId)
                .Select(u => new UserProfileDto
                {
                    FullName = u.FullName,
                    Address = new AddressDto { MainAddress = u.Address.MainAddress, SecondaryAddress = u.Address.SecondaryAddress, Street = u.Address.Street },
                    PhoneNumber = u.PhoneNumber,
                    Email = u.Email,
                    CityName = u.City.Name,
                    GovernmentName = u.City.Government.Name,
                    CountryName = u.City.Government.Country.Name,
                    BundleName = u.Subscriptions
                        .Select(s => s.Bundle.BundleName)
                        .FirstOrDefault(),
                    TotalShipmentsSent = u.SentShipments.Count()
                })
               .FirstOrDefaultAsync(ct);
        

        public async Task<IEnumerable<UserProfileDto>> GetUsersAsync(CancellationToken ct)
            => await context.UserProfiles
                .AsNoTracking()
                .Select(u => new UserProfileDto
                {
                    FullName = u.FullName,
                    Address = new AddressDto { MainAddress = u.Address.MainAddress, SecondaryAddress = u.Address.SecondaryAddress, Street = u.Address.Street },
                    PhoneNumber = u.PhoneNumber,
                    Email = u.Email,
                    CityName = u.City.Name,
                    BundleName = u.Subscriptions
                        .Select(s => s.Bundle.BundleName)
                        .FirstOrDefault(),
                    TotalShipmentsSent = u.SentShipments.Count()
                })
               .ToListAsync(ct);

        public async Task<AdminUserDetailsDto?> GetUserDetailsForAdminAsync(Guid userId, CancellationToken ct)
        {
            var now = DateTimeOffset.UtcNow;

            return await BuildAdminUsersQuery(now)
                .Where(user => user.UserId == userId)
                .FirstOrDefaultAsync(ct);
        }

        public async Task<PagedResult<AdminUserDetailsDto>> FilterUsersAsync(UserFiltrationDto filter, CancellationToken ct)
        {
            var pageNumber = filter.PageNumber <= 0 ? 1 : filter.PageNumber;
            var pageSize = filter.PageSize <= 0 ? 20 : filter.PageSize;
            var query = from profile in context.UserProfiles.AsNoTracking()
                        join appUser in context.AppUsers.AsNoTracking()
                         on profile.AppUserId equals appUser.Id
                        select new
                        {
                            Profile = profile,
                            AppUser = appUser
                        };

            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                var searchTerm = filter.SearchTerm.Trim();
                query = query.Where(user =>
                    user.Profile.FirstName.Contains(searchTerm) ||
                    user.Profile.LastName.Contains(searchTerm) ||
                    user.Profile.Email.Contains(searchTerm) ||
                    user.Profile.PhoneNumber.Contains(searchTerm) ||
                    user.Profile.Address.MainAddress.Contains(searchTerm) ||
                    (user.Profile.Address.SecondaryAddress != null && user.Profile.Address.SecondaryAddress.Contains(searchTerm)) ||
                    user.Profile.Address.Street.Contains(searchTerm) ||
                    (user.AppUser.UserName != null && user.AppUser.UserName.Contains(searchTerm)));
            }

            if (!string.IsNullOrWhiteSpace(filter.Email))
            {
                var email = filter.Email.Trim();
                query = query.Where(user => user.Profile.Email.Contains(email));
            }

            if (!string.IsNullOrWhiteSpace(filter.UserName))
            {
                var userName = filter.UserName.Trim();
                query = query.Where(user => user.AppUser.UserName != null && user.AppUser.UserName.Contains(userName));
            }

            if (!string.IsNullOrWhiteSpace(filter.PhoneNumber))
            {
                var phoneNumber = filter.PhoneNumber.Trim();
                query = query.Where(user => user.Profile.PhoneNumber.Contains(phoneNumber));
            }

            if (filter.IsActive.HasValue)
            {
                query = query.Where(user => user.Profile.CurrentState == filter.IsActive.Value);
            }

            if (filter.CreatedFrom.HasValue)
            {
                query = query.Where(user => user.Profile.CreatedAt >= filter.CreatedFrom.Value);
            }

            if (filter.CreatedTo.HasValue)
            {
                query = query.Where(user => user.Profile.CreatedAt <= filter.CreatedTo.Value);
            }

            var totalCount = await query.CountAsync(ct);

            var users = await query
                .OrderByDescending(user => user.Profile.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(user => new AdminUserDetailsDto
                {
                    UserId = user.Profile.AppUserId,
                    ProfileId = user.Profile.Id,
                    UserName = user.AppUser.UserName ?? string.Empty,
                    FullName = user.Profile.FirstName + " " + user.Profile.LastName,
                    Email = user.Profile.Email,
                    PhoneNumber = user.Profile.PhoneNumber,
                    Address = new AddressDto { MainAddress = user.Profile.Address.MainAddress, SecondaryAddress = user.Profile.Address.SecondaryAddress, Street = user.Profile.Address.Street },
                    UserType = user.Profile.UserType,
                    CityName = user.Profile.City.Name,
                    GovernmentName = user.Profile.City.Government.Name,
                    CountryName = user.Profile.City.Government.Country.Name,
                    BundleName = user.Profile.Subscriptions
                        .Where(subscription => subscription.IsActive)
                        .Select(subscription => subscription.Bundle.BundleName)
                        .FirstOrDefault(),
                    TotalShipmentsSent = user.Profile.SentShipments.Count(),
                    IsActive = user.Profile.CurrentState,
                    CreatedAt = user.Profile.CreatedAt
                })
                .ToListAsync(ct);

            return PagedResult<AdminUserDetailsDto>.From(users, totalCount, pageNumber, pageSize);
        }

         public async Task<RetrieveShipmentDto?> GetUserShipmentDetailsAsync(Guid AppUserId,Guid shipmentId, CancellationToken ct)
           => await context.Shipments
            .AsNoTracking()
            .Where(sh => sh.Id == shipmentId && sh.Sender.AppUserId == AppUserId)
            .ProjectTo<RetrieveShipmentDto>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(ct);

        private IQueryable<AdminUserDetailsDto> BuildAdminUsersQuery(DateTimeOffset now)
            =>
                from profile in context.UserProfiles.AsNoTracking()
                join appUser in context.AppUsers.AsNoTracking()
                    on profile.AppUserId equals appUser.Id
                select new AdminUserDetailsDto
                {
                    UserId = profile.AppUserId,
                    ProfileId = profile.Id,
                    UserName = appUser.UserName ?? string.Empty,
                    FullName = profile.FirstName + " " + profile.LastName,
                    Email = profile.Email,
                    PhoneNumber = profile.PhoneNumber,
                    Address = new AddressDto { MainAddress = profile.Address.MainAddress, SecondaryAddress = profile.Address.SecondaryAddress, Street = profile.Address.Street },
                    UserType = profile.UserType,
                    CityName = profile.City.Name,
                    GovernmentName = profile.City.Government.Name,
                    CountryName = profile.City.Government.Country.Name,
                    BundleName = profile.Subscriptions
                        .Where(subscription => subscription.IsActive)
                        .Select(subscription => subscription.Bundle.BundleName)
                        .FirstOrDefault(),
                    TotalShipmentsSent = profile.SentShipments.Count(),
                    IsActive = profile.CurrentState,
                    IsLockedOut = appUser.LockoutEnd.HasValue && appUser.LockoutEnd > now,
                    CreatedAt = profile.CreatedAt
                };

        public async Task<string?> GetUserFullName(Guid AppUserId, CancellationToken ct)
          => await context.UserProfiles
                .AsNoTracking()
                .Where(u => u.AppUserId == AppUserId || u.Id == AppUserId)
                .Select(u => u.FullName)
                .FirstOrDefaultAsync(ct);
    }
}
