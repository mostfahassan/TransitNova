namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.CarrierCompanies.Queries;

public interface IGetCarrierCompanyByIdQueryService
{
    const string HttpMethod = "GET";
    const string Route = "api/v{version:apiVersion}/admins/carriers/companies/{companyId:guid}";

    Task<ApiResponse<UiRetrieveCarrierCompany?>> GetCarrierCompanyByIdAsync(Guid companyId, CancellationToken cancellationToken = default);
}

