using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.ZoneDtos;
using TransitNova.BusinessLayer.Features.Zones.Commands;
using TransitNova.BusinessLayer.Interfaces.Repositories.LocationRepository;
using TransitNova.BusinessLayer.Interfaces.Services.UnitOfWork;
using TransitNova.Domain.Entities.MainEntities;
namespace TransitNova.BusinessLayer.Features.Zones.Handlers.ApplyCommands
{
    public sealed class CreateZoneHandler(
        IZoneRepository repository,
        IUnitOfWork unitOfWork)
        : ICommandHandler<CreateZoneCommand, Result<ZoneDto>>
    {
        public async Task<Result<ZoneDto>> Handle(CreateZoneCommand request, CancellationToken ct)
        {
            
            Zone zone = Zone.Create(request.Dto.Name.Trim(), request.Dto.CityId);
            await repository.AddAsync(zone, ct);
            await unitOfWork.SaveChangesAsync(ct);

            var dto = await repository.GetByIdAsync<ZoneDto>(zone.Id, ct);
            return dto is null
                ? Result<ZoneDto>.Failure(Errors.ZoneNotFound("Zone created but could not be retrieved."))
                : Result<ZoneDto>.Created(dto);
        }   
    }
}
