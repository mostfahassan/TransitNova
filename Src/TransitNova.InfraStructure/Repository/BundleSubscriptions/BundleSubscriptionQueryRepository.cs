using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TransitNova.BusinessLayer.Common.CommonData;
using TransitNova.BusinessLayer.DTOs.BundleSubscription;
using TransitNova.BusinessLayer.DTOs.UserProfile;
using TransitNova.BusinessLayer.Interfaces.Repositories.BundleSubscriptionRepository;
using TransitNova.Domain.Contracts.Constants;
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

        public async Task<IEnumerable<BundleSubscriptionDetailsDto>> GetSubscribersAsync(CancellationToken ct)
        {
            return await context.UserBundleSubscriptions
                .AsNoTracking()
                .OrderByDescending(x => x.SubscriptionDate)
                .Select(SubscriptionProjection)
                .ToListAsync(ct);
        }

        public async Task<IEnumerable<BundleSubscriptionDetailsDto>> GetBundleSubscribersAsync(Guid bundleId, CancellationToken ct)
        {
            return await context.UserBundleSubscriptions
                .AsNoTracking()
                .Where(x => x.BundleId == bundleId)
                .OrderByDescending(x => x.SubscriptionDate)
                .Select(SubscriptionProjection)
                .ToListAsync(ct);
        }

        public async Task<ActiveBundleSubscriptionBenefitDto?> GetActiveSubscriptionForUserAsync(Guid userId, CancellationToken ct)
        {
            var now = DateTime.UtcNow;

            return await context.UserBundleSubscriptions
                .AsNoTracking()
                .Where(x => x.SubscribedUserId == userId &&
                            x.IsActive &&
                            (x.EndDate == null || x.EndDate > now))
                .OrderByDescending(x => x.SubscriptionDate)
                .Select(x => new ActiveBundleSubscriptionBenefitDto
                {
                    SubscriptionId = x.Id,
                    BundleId = x.BundleId,
                    BundleName = x.Bundle.BundleName,
                    SubscriptionDate = x.SubscriptionDate,
                    EndDate = x.EndDate,
                    MaxShipmentsPerMonth = x.Bundle.MaxShipmentsPerMonth,
                    MaxWeightPerShipment = x.Bundle.MaxWeightPerShipment,
                    MaxDistancePerShipment = x.Bundle.MaxDistancePerShipment,
                    DiscountPercentage = x.Bundle.DiscountPercentage,
                    MinimumShipmentValueForDiscount = x.Bundle.MinimumShipmentValueForDiscount
                })
                .FirstOrDefaultAsync(ct);
        }

        public async Task<int> GetMonthlyAppliedBenefitCountAsync(Guid userId, Guid bundleId, DateTime monthStartUtc, DateTime monthEndUtc, CancellationToken ct)
        {
            return await context.PaymentInvoices
                .AsNoTracking()
                .CountAsync(invoice => invoice.CustomerId == userId &&
                    invoice.BundleId == bundleId &&
                    invoice.SubscriptionBenefitApplied &&
                    invoice.ReferecneType == Constant.PaymentReferenceConstants.Shipment &&
                    invoice.CreatedAt >= monthStartUtc &&
                    invoice.CreatedAt < monthEndUtc,
                    ct);
        }

        static Expression<Func<BundleSubscription, BundleSubscriptionDetailsDto>> SubscriptionProjection
             => x => new BundleSubscriptionDetailsDto
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
             };

        static Expression<Func<BundleSubscription, UserProfileDto>> UserProfileProjection
             => x => new UserProfileDto
             {
                 Id = x.SubscribedUser.AppUserId,
                 FullName = x.SubscribedUser.FullName,
                 Address = new AddressDto
                 {
                     MainAddress = x.SubscribedUser.Address.MainAddress,
                     SecondaryAddress = x.SubscribedUser.Address.SecondaryAddress,
                     Street = x.SubscribedUser.Address.Street
                 },
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