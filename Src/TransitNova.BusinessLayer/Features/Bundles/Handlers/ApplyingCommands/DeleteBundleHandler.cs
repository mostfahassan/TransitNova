using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.Caching;
using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Features.Bundles.Commands;
using TransitNova.Domain.Entities.MainEntities;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.Interfaces.Repositories.GenericRepository;
using TransitNova.BusinessLayer.Interfaces.Services.UnitOfWork;
using TransitNova.Domain.Contracts.Caching;
namespace TransitNova.BusinessLayer.Features.Bundles.Handlers.ApplyingCommands
{
    public sealed class DeleteBundleHandler(
    IGenericRepository<Bundle, Guid> repository,
    IUnitOfWork unitOfWork,
    ILogger<DeleteBundleHandler> logger)
    : ICommandHandler<DeleteBundleCommand, BaseResult>
    {
        public async Task<BaseResult> Handle(DeleteBundleCommand request, CancellationToken ct)
        {
            await repository.DeleteAsync(request.Id, ct);

            await unitOfWork.SaveChangesAsync(ct);

            logger.LogInformation("Bundle deleted successfully. Id: {BundleId}", request.Id);
            CacheInvalidationContext.Set(request, CacheKeys.Bundles.List, CacheKeys.Bundles.ById(request.Id));
            return BaseResult.Success();
        }
    }

}


