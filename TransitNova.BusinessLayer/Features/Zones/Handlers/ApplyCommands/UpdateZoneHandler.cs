using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.Features.Zones.Commands;
using TransitNova.BusinessLayer.Interfaces.Repositories.LocationRepository;
using TransitNova.BusinessLayer.Interfaces.Services.UnitOfWork;
using TransitNova.Domain.DomainExceptions;
using TransitNova.Domain.Entities.MainEntities;

namespace TransitNova.BusinessLayer.Features.Zones.Handlers.ApplyCommands
{
    public sealed class UpdateZoneHandler(
        IZoneRepository repository,
        ILogger<UpdateZoneHandler> logger,
        IUnitOfWork unitOfWork)
        : ICommandHandler<UpdateZoneCommand, BaseResult>
    {
        public async Task<BaseResult> Handle(UpdateZoneCommand request, CancellationToken ct)
        {
            var entity = await repository.GetByIdAsync<Zone>(request.Dto.ZoneId, ct);
            if (entity == null)
            {
                logger.LogInformation("No City Found With Id {CountryId}", request.Dto.ZoneId);
                throw new EntityNotFoundException($"No Zone With ID:{request.Dto.ZoneId} Has Been Founded", "Zone_NOT_FOUNDED", nameof(Zone));
            }
            entity.Update(request.Dto.Name, request.Dto.Code, request.Dto.CityId);
            repository.Update(entity);
            await unitOfWork.SaveChangesAsync(ct);
            return BaseResult.Success();
        }
    }
}
