
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
    public sealed class UpdateCarrierCompanyHandler(
        IGenericRepository<CarrierCompany, Guid> repository,
        IUnitOfWork unitOfWork,
        IOperationManagerQueryRepository operationManager,
        ICacheService cacheService,
        ILogger<UpdateCarrierCompanyHandler> logger)
        : ICommandHandler<UpdateCarrierCompanyCommand, BaseResult>
    {
        public async Task<BaseResult> Handle(UpdateCarrierCompanyCommand request, CancellationToken ct)
        {
            var userId = (await operationManager.GetUserIdAsync(request.AdminId, ct)).ToString();
            if (string.IsNullOrEmpty(userId))
            {
                return Result<RetrieveCarrierCompany>.NotFound(Errors.UserNotFound("Operation Manager Not Found "));
            }

            var entity = await repository.GetByIdAsync<CarrierCompany>(request.Id, ct);
            if (entity == null)
            {
                return Result<RetrieveCarrierCompany>.NotFound(Errors.NotFound("Carrier Company Not Found"));
            }


            entity.Update(request.Dto.Name!, request.Dto.Email, request.Dto.PhoneNumber, request.Dto.Address, request.Dto.ZoneId, userId);
            repository.Update(entity);
          
            await unitOfWork.SaveChangesAsync(ct);
       

            logger.LogInformation("CarrierCompany updated successfully. Id: {CompanyId}", request.Id);
            await cacheService.RemoveAsync(CacheKeys.CarrierCompanyList());
            await cacheService.RemoveAsync(CacheKeys.CarrierCompanyById(request.Id));
            return BaseResult.Success();
        }
    }
}
