namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Subscriptions.Queries;

public interface IGetSubscriptionByIdQueryService
{
    Task<ApiResponse<UiBundleSubscriptionDetailsDto>> GetSubscriptionByIdAsync(Guid subscriptionId, string bearerToken, CancellationToken cancellationToken = default);
}

