using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.BundleSubscription;

namespace TransitNova.BusinessLayer.Features.BundleSubscriptions.Queries;

public sealed record GetSubscribersQuery : IQuery<Result<List<BundleSubscriptionDetailsDto>>>;