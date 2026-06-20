using AutoMapper;
using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.Caching;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.CarrierCompany;
using TransitNova.BusinessLayer.Features.CarrierCompanies.Commands;
using TransitNova.BusinessLayer.Interfaces.Repositories.GenericRepository;
using TransitNova.BusinessLayer.Interfaces.Repositories.OperationManagerRepository;
using TransitNova.BusinessLayer.Interfaces.Services.CacheService;
using TransitNova.BusinessLayer.Interfaces.Services.UnitOfWork;
using TransitNova.Domain.Entities.MainEntities;
namespace TransitNova.BusinessLayer.Features.CarrierCompanies.Handlers.ApplyingCommands
{
    public sealed class CreateCarrierCompanyHandler(
    IGenericRepository<CarrierCompany, Guid> repository,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IOperationManagerQueryRepository operationManager,
    ICacheService cacheService,
    ILogger<CreateCarrierCompanyHandler> logger)
    : ICommandHandler<CreateCarrierCompanyCommand, Result<RetrieveCarrierCompany>>
    {
        public async Task<Result<RetrieveCarrierCompany>> Handle(CreateCarrierCompanyCommand request, CancellationToken ct)
        {
            var userId = (await operationManager.GetUserIdAsync(request.AdminId,ct)).ToString();
            if (string.IsNullOrEmpty(userId))
            {
                return Result<RetrieveCarrierCompany>.NotFound(Errors.UserNotFound("Operation Manager Not Found "));
            }
            var carrierCompany = CarrierCompany.Create(userId.ToString(), request.Dto.Name, request.Dto.Email, request.Dto.PhoneNumber, request.Dto.Address, request.Dto.ZoneId);

            await repository.AddAsync(carrierCompany, ct);
       
            await unitOfWork.SaveChangesAsync(ct);
          
            logger.LogInformation("CarrierCompany created successfully. Id: {CompanyId}", carrierCompany.Id);
            await cacheService.RemoveAsync(CacheKeys.CarrierCompanyList());
            return Result<RetrieveCarrierCompany>.Success(mapper.Map<RetrieveCarrierCompany>(carrierCompany));
        }
    }
}
