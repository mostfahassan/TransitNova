namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Subscriptions.Queries;

public interface IGetSubscriptionByIdQueryService
{
    const string HttpMethod = "GET";
    const string Route = "api/v{version:apiVersion}/admin/subscriptions/{subscriptionId:guid}";

    Task<ApiResponse<UiBundleSubscriptionDetailsDto>> GetSubscriptionByIdAsync(Guid subscriptionId, CancellationToken cancellationToken = default);
}

