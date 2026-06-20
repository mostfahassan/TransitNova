namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.CarrierCompanies.Commands;

public interface IDeleteCarrierCompanyCommandService
{
    const string HttpMethod = "DELETE";
    const string Route = "api/v{version:apiVersion}/admins/carriers/companies/{companyId:guid}";

    Task<ApiResponse> DeleteCarrierCompanyAsync(Guid companyId, CancellationToken cancellationToken = default);
}

