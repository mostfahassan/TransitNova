using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.Features.Zones.Commands;
using TransitNova.BusinessLayer.Interfaces.Repositories.LocationRepository;
using TransitNova.BusinessLayer.Interfaces.Services.UnitOfWork;
namespace TransitNova.BusinessLayer.Features.Zones.Handlers.ApplyCommands
{
    public sealed class DeleteZoneHandler(IZoneRepository repository, IUnitOfWork unitOfWork)
        : ICommandHandler<DeleteZoneCommand, BaseResult>
    {
        public async Task<BaseResult> Handle(DeleteZoneCommand request, CancellationToken ct)
        {
            await repository.DeleteAsync(request.Id, ct);
            await unitOfWork.SaveChangesAsync(ct);
            return BaseResult.Success();
        }
    }
}
