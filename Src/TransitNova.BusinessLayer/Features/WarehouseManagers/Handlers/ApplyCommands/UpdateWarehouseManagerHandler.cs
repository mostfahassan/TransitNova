
using TransitNova.BusinessLayer.Common.Caching;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.Features.WarehouseManagers.Commands;
using TransitNova.BusinessLayer.Interfaces.Repositories.WarehouseManagerRepository;
using TransitNova.BusinessLayer.Interfaces.Services.UnitOfWork;
using TransitNova.Domain.Contracts.Caching;
namespace TransitNova.BusinessLayer.Features.WarehouseManagers.Handlers.ApplyCommands
{
    public sealed class UpdateWarehouseManagerHandler(
        IUnitOfWork unitOfWork,
        IWarehouseManagerCommandRepository commandRepository)
        : ICommandHandler<UpdateWarehouseManagerCommand, BaseResult>
    {
        public async Task<BaseResult> Handle(UpdateWarehouseManagerCommand request, CancellationToken ct)
        {
           var manager =  await commandRepository.GetByIdForUpdateAsync(request.Dto.Id, ct);
            if (manager is null)
                return BaseResult.NotFound(Errors.NotFound("Warehouse manager not found."));

            manager.Update(request.Dto.FirstName, request.Dto.LastName, request.Dto.PhoneNumber, request.Dto.Email, request.Dto.CityId, request.Dto.Address?.ToDomain());
            await unitOfWork.SaveChangesAsync(ct);

            CacheInvalidationContext.Set(request,CacheKeys.WarehouseManagers.Details(request.Dto.Id), CacheKeys.WarehouseManagers.Dashboard(request.Dto.Id));
            return BaseResult.NoContent();
        }
    }
}
