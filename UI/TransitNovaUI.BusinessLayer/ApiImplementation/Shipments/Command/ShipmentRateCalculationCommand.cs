using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Shipments.Commands;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.User.Pricing.Segregation;
using TransitNovaUI.BusinessLayer.Common.APIHelper.Http;

namespace TransitNovaUI.BusinessLayer.ApiImplementation.Shipments.Command
{
    public class ShipmentRateCalculationCommand(IHttpHandler httpHandler, HttpClient httpClient) : ApiServiceBase(httpHandler, httpClient), IUserPricingCommand
    {
        public Task<ApiResponse<decimal>> CalculateRateAsync(UiRateCalculatorDto model, CancellationToken cancellationToken = default)
        {
            var content = UiRateCalculatorDto.ToDto(model);
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.Shipments.RateCalculationUrl));

            return SendRequestAsync<decimal>(HttpMethod.Post, url, null, cancellationToken, content);
        }
    }
}


