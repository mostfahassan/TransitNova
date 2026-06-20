using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.CarrierCompany;
using TransitNova.BusinessLayer.Features.CarrierCompanies.Queries;
using TransitNova.BusinessLayer.Interfaces.Repositories.GenericRepository;
using TransitNova.Domain.Entities.MainEntities;
using TransitNova.Domain.Enums.Result;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TransitNova.BusinessLayer.Features.CarrierCompanies.Handlers.ApplyingQueries
{
    // --- Features/CarrierCompany/Handlers/Queries/GetCarrierCompanyByIdHandler.cs ---
    public sealed class GetCarrierCompanyByIdHandler(
        IGenericRepository<CarrierCompany, Guid> repository,
        ILogger<GetCarrierCompanyByIdHandler> logger)
        : IQueryHandler<GetCarrierCompanyByIdQuery, Result<RetrieveCarrierCompany?>>
    {
        public async Task<Result<RetrieveCarrierCompany?>> Handle(GetCarrierCompanyByIdQuery request, CancellationToken ct)
        {
            var dto = await repository.GetByIdAsync<RetrieveCarrierCompany>(request.Id, ct);
            if (dto is null)
            {
                logger.LogWarning("CarrierCompany not found. Id: {CompanyId}", request.Id);
                return Result<RetrieveCarrierCompany?>.NotFound(Errors.NotFound("CarrierCompany not found."));
            }

            return Result<RetrieveCarrierCompany?>.Success(dto);
        }
    }
}