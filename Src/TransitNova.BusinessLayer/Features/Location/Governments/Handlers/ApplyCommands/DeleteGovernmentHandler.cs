using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.Features.Location.Governments.Commands;
using TransitNova.BusinessLayer.Interfaces.Repositories.GenericRepository;
using TransitNova.BusinessLayer.Interfaces.Services.UnitOfWork;
using TransitNova.Domain.Entities.MainEntities;
namespace TransitNova.BusinessLayer.Features.Location.Governments.Handlers.ApplyCommands
{
    public sealed class DeleteGovernmentHandler(
        IGenericRepository<Government, int> repository,
        IUnitOfWork unitOfWork,
        ILogger<DeleteGovernmentHandler> logger)
        : ICommandHandler<DeleteGovernmentCommand, BaseResult>
    {
        public async Task<BaseResult> Handle(DeleteGovernmentCommand request, CancellationToken ct)
        {

            var government = await repository.GetByIdAsync<Government>(request.Id, ct);
            if (government == null)
                return BaseResult.NotFound(Errors.NotFound("City Not Found"));

            var deleted = await repository.DeleteAsync(request.Id, ct);
            if (!deleted)
            {
                logger.LogWarning("Government delete failed because government was not found. GovernmentId: {GovernmentId}", request.Id);
                return BaseResult.NotFound(Errors.NotFound("Government not found."));
            }

            await unitOfWork.SaveChangesAsync(ct);

            logger.LogInformation("Government deleted successfully. GovernmentId: {GovernmentId}", request.Id);
            return BaseResult.Success();
        }
    }
}
