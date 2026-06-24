using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.Features.Location.Governments.Commands;
using TransitNova.BusinessLayer.Interfaces.Repositories.GenericRepository;
using TransitNova.BusinessLayer.Interfaces.Services.UnitOfWork;
using TransitNova.Domain.Entities.MainEntities;

namespace TransitNova.BusinessLayer.Features.Location.Governments.Handlers.ApplyCommands
{
    public sealed class UpdateGovernmentHandler(
        IGenericRepository<Government, int> governmentRepository,
        IGenericRepository<Country, int> countryRepository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateGovernmentHandler> logger)
        : ICommandHandler<UpdateGovernmentCommand, BaseResult>
    {
        public async Task<BaseResult> Handle(UpdateGovernmentCommand request, CancellationToken ct)
        {
            var government = await governmentRepository.GetByIdAsync<Government>(request.Id, ct);
            if (government is null)
            {
                logger.LogWarning("Government update failed because government was not found. GovernmentId: {GovernmentId}", request.Id);
                return BaseResult.NotFound(Errors.NotFound("Government not found."));
            }

            var countryExists = await countryRepository.ExistsAsync(request.CountryId, ct);
            if (!countryExists)
            {
                logger.LogWarning("Government update failed because country was not found. GovernmentId: {GovernmentId}, CountryId: {CountryId}", request.Id, request.CountryId);
                return BaseResult.NotFound(Errors.NotFound("Country not found."));
            }

            government.Update(request.Name.Trim(), request.CountryId);
            governmentRepository.Update(government);
            await unitOfWork.SaveChangesAsync(ct);

            logger.LogInformation("Government updated successfully. GovernmentId: {GovernmentId}, CountryId: {CountryId}", request.Id, request.CountryId);
            return BaseResult.Success();
        }
    }
}
