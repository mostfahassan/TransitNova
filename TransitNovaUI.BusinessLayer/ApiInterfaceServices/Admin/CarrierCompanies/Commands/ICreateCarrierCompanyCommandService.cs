namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.CarrierCompanies.Commands;

public interface ICreateCarrierCompanyCommandService
{
    const string HttpMethod = "POST";
    const string Route = "api/v{version:apiVersion}/admins/carriers/companies";

    Task<ApiResponse<UiRetrieveCarrierCompany>> CreateCarrierCompanyAsync(UiAddCarrierCompany request, CancellationToken cancellationToken = default);
}

