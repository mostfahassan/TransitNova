using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.CarrierCompany;
using TransitNova.BusinessLayer.Features.CarrierCompanies.Queries;
using TransitNova.BusinessLayer.Interfaces.Repositories.GenericRepository;
using TransitNova.Domain.Entities.MainEntities;
namespace TransitNova.BusinessLayer.Features.CarrierCompanies.Handlers.ApplyingQueries
{
    public sealed class GetCarrierCompanyListHandler(
        IGenericRepository<CarrierCompany, Guid> repository,
        ILogger<GetCarrierCompanyListHandler> logger)
        : IQueryHandler<GetCarrierCompanyListQuery, Result<List<RetrieveCarrierCompany>>>
    {
        public async Task<Result<List<RetrieveCarrierCompany>>> Handle(GetCarrierCompanyListQuery request, CancellationToken ct)
        {
            var list = await repository.GetListAsync<RetrieveCarrierCompany>(ct);

            if (list.Count == 0)
            {
                logger.LogInformation("No CarrierCompanies found.");
                var emptyResult = Result<List<RetrieveCarrierCompany>>.Success([]);
                return emptyResult;
            }

            var result = Result<List<RetrieveCarrierCompany>>.Success(list);
            return result;
        }
    }
}
