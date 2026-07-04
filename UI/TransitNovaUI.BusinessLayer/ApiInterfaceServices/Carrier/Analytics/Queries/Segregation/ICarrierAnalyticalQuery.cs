namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Carrier.Analytics.Queries.Segregation
{
    public interface ICarrierAnalyticsQuery : IGetCarrierRatingQueryService, IGetCarrierRevenueQueryService
    {
    }

    public interface ICarrierAnalyticalQuery : ICarrierAnalyticsQuery
    {
    }
}
