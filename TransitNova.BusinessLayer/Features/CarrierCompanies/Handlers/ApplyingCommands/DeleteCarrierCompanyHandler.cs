using TransitNova.BusinessLayer.Common.CQRS;
using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.Caching;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.Features.CarrierCompanies.Commands;
using TransitNova.BusinessLayer.Interfaces.Services.CacheService;
using TransitNova.Domain.Entities.MainEntities;
using TransitNova.BusinessLayer.Interfaces.Repositories.GenericRepository;
using TransitNova.BusinessLayer.Interfaces.Services.UnitOfWork;

namespace TransitNova.BusinessLayer.Features.CarrierCompanies.Handlers.ApplyingCommands
{
    public sealed class DeleteCarrierCompanyHandler(
        IGenericRepository<CarrierCompany, Guid> repository,
        IUnitOfWork unitOfWork,
        ICacheService cacheService,
        ILogger<DeleteCarrierCompanyHandler> logger)
        : ICommandHandler<DeleteCarrierCompanyCommand, BaseResult>
    {
        public async Task<BaseResult> Handle(DeleteCarrierCompanyCommand request, CancellationToken ct)
        {
            await repository.DeleteAsync(request.Id, ct);

            await unitOfWork.SaveChangesAsync(ct);

            logger.LogInformation("CarrierCompany deleted successfully. Id: {CompanyId}", request.Id);
            await cacheService.RemoveAsync(CacheKeys.CarrierCompanyList());
            await cacheService.RemoveAsync(CacheKeys.CarrierCompanyById(request.Id));
            return BaseResult.Success();
        }
    }
}
