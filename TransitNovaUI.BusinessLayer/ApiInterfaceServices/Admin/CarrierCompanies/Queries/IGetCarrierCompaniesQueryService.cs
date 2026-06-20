namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.CarrierCompanies.Queries;

public interface IGetCarrierCompaniesQueryService
{
    const string HttpMethod = "GET";
    const string Route = "api/v{version:apiVersion}/admins/carriers/companies";

    Task<ApiResponse<List<UiRetrieveCarrierCompany>>> GetCarrierCompaniesAsync(CancellationToken cancellationToken = default);
}

