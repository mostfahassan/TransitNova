using TransitNova.BusinessLayer.Common.CQRS;
using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.Caching;
using TransitNova.BusinessLayer.Features.Bundles.Commands;
using TransitNova.BusinessLayer.Interfaces.Services.CacheService;
using TransitNova.Domain.Entities.MainEntities;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.Interfaces.Repositories.GenericRepository;
using TransitNova.BusinessLayer.Interfaces.Services.UnitOfWork;
namespace TransitNova.BusinessLayer.Features.Bundles.Handlers.ApplyingCommands
{
    public sealed class DeleteBundleHandler(
    IGenericRepository<Bundle, int> repository,
    IUnitOfWork unitOfWork,
    ICacheService cacheService,
    ILogger<DeleteBundleHandler> logger)
    : ICommandHandler<DeleteBundleCommand, BaseResult>
    {
        public async Task<BaseResult> Handle(DeleteBundleCommand request, CancellationToken ct)
        {
            await repository.DeleteAsync(request.Id, ct);

            await unitOfWork.SaveChangesAsync(ct);

            logger.LogInformation("Bundle deleted successfully. Id: {BundleId}", request.Id);
            await cacheService.RemoveAsync(CacheKeys.BundleList());
            await cacheService.RemoveAsync(CacheKeys.BundleById(request.Id));
            return BaseResult.Success();
        }
    }

}
