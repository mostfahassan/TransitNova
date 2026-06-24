using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.UserProfile;

namespace TransitNova.BusinessLayer.Features.BundleSubscriptions.Queries
{
    public sealed record GetBundleSubscribersQuery(Guid BundleId)
        : IQuery<Result<List<UserProfileDto>>>;
}
