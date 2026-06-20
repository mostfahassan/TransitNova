namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.CarrierCompanies.Commands;

public interface IUpdateCarrierCompanyCommandService
{
    const string HttpMethod = "PUT";
    const string Route = "api/v{version:apiVersion}/admins/carriers/companies/{companyId:guid}";

    Task<ApiResponse> UpdateCarrierCompanyAsync(Guid companyId, UiUpdateCarrierCompany request, CancellationToken cancellationToken = default);
}

