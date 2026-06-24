using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.Features.Zones.Commands;
using TransitNova.BusinessLayer.Interfaces.Repositories.LocationRepository;
using TransitNova.BusinessLayer.Interfaces.Services.UnitOfWork;
using TransitNova.Domain.Entities.MainEntities;
namespace TransitNova.BusinessLayer.Features.Zones.Handlers.ApplyCommands
{
    public sealed class DeleteZoneHandler(IZoneRepository repository, IUnitOfWork unitOfWork,ILogger<DeleteZoneHandler> logger)
        : ICommandHandler<DeleteZoneCommand, BaseResult>
    {
        public async Task<BaseResult> Handle(DeleteZoneCommand request, CancellationToken ct)
        {
            var zone = await repository.GetByIdAsync<Zone>(request.Id, ct);
            if (zone == null)
                return BaseResult.NotFound(Errors.NotFound("Zone Not Found"));
            var deleted = await repository.DeleteAsync(request.Id, ct);
            if (!deleted)
            {
                logger.LogWarning("Zone delete failed because Zone was not found.ZoneId: {ZoneId}", request.Id);
                return BaseResult.UnExpected(Errors.FailedOperation("An error occurred while removing the zone."));
            }
            await unitOfWork.SaveChangesAsync(ct);
            return BaseResult.Success();
        }
    }
}
