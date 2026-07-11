
using Microsoft.EntityFrameworkCore;
using TransitNova.BusinessLayer.Common.CommonData;
using System.Linq.Expressions;
using TransitNova.BusinessLayer.DTOs.BundleSubscription;
using TransitNova.BusinessLayer.DTOs.UserProfile;
using TransitNova.BusinessLayer.Interfaces.Repositories.BundleSubscriptionRepository;
using TransitNova.Domain.Entities.MainEntities;
using TransitNova.InfraStructure.Context;
namespace TransitNova.InfraStructure.Repository.BundleSubscriptions
{
    internal class BundleSubscriptionQueryRepository(AppDbContext context) : IBundleSubscriptionQueryRepository
    {
        public async Task<BundleSubscriptionDetailsDto?> GetSubscriptionDetailsAsync(Guid subscriptionId, CancellationToken ct)
        {
            return await context.UserBundleSubscriptions
                .AsNoTracking()
                .Where(x => x.Id == subscriptionId)
                .Select(x => new BundleSubscriptionDetailsDto
                {
                    Id = x.Id,
                    BundleId = x.BundleId,
                    BundleName = x.Bundle.BundleName,
                    UserId = x.SubscribedUserId,
                    FullName = x.SubscribedUser.FullName,
                    Email = x.SubscribedUser.Email,
                    PhoneNumber = x.SubscribedUser.PhoneNumber,
                    IsActive = x.IsActive,
                    SubscriptionDate = x.SubscriptionDate,
                    EndDate = x.EndDate,
                    CancelledAt = x.CancelledAt
                })
                .FirstOrDefaultAsync(ct);
        }

        public async Task<UserProfileDto?> GetSubscribedUserAsync(Guid userId, Guid bundleId, CancellationToken ct)
        {
            return await context.UserBundleSubscriptions
                .AsNoTracking()
                .Where(x => x.SubscribedUserId == userId &&
                            x.BundleId == bundleId &&
                            x.IsActive)
                .Select(UserProfileProjection)
                .FirstOrDefaultAsync(ct);
        }
        public async Task<IEnumerable<UserProfileDto>> GetSubscribedUsersAsync(Guid bundleId, CancellationToken ct)
        {
            return await context.UserBundleSubscriptions
                .AsNoTracking()
                .Where(x => x.BundleId == bundleId && x.IsActive)
                .Select(UserProfileProjection)
                .ToListAsync(ct);
        }

        static Expression<Func<BundleSubscription, UserProfileDto>> UserProfileProjection
             => x => new UserProfileDto
             {
                 FullName = x.SubscribedUser.FullName,
                 Address = new AddressDto { MainAddress = x.SubscribedUser.Address.MainAddress, SecondaryAddress = x.SubscribedUser.Address.SecondaryAddress, Street = x.SubscribedUser.Address.Street },
                 PhoneNumber = x.SubscribedUser.PhoneNumber,
                 Email = x.SubscribedUser.Email,

                 CityName = x.SubscribedUser.City.Name,
                 GovernmentName = x.SubscribedUser.City.Government.Name,
                 CountryName = x.SubscribedUser.City.Government.Country.Name,

                 BundleName = x.Bundle.BundleName,

                 TotalShipmentsSent = x.SubscribedUser.SentShipments.Count()
             };
    }
}
