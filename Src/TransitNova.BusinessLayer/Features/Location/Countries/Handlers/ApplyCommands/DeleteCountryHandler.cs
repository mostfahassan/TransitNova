using TransitNova.BusinessLayer.Common.CQRS;
using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.Features.Location.Countries.Commands;
using TransitNova.BusinessLayer.Interfaces.Repositories.LocationRepository;
using TransitNova.BusinessLayer.Interfaces.Services.UnitOfWork;
using TransitNova.Domain.Entities.MainEntities;

namespace TransitNova.BusinessLayer.Features.Location.Countries.Handlers.ApplyCommands
{
    public sealed class DeleteCountryHandler(
        ICountryRepository repository,
        IUnitOfWork unitOfWork,
        ILogger<DeleteCountryHandler> logger)
        : ICommandHandler<DeleteCountryCommand, BaseResult>
    {
        public async Task<BaseResult> Handle(DeleteCountryCommand request, CancellationToken ct)
        {
            var country = await repository.GetByIdAsync<Country>(request.Id,ct);
            if (country == null)
                return BaseResult.NotFound(Errors.NotFound("Country Not Found"));


            var deleted =  await repository.DeleteAsync(request.Id, ct);
            if (!deleted)
            {
                logger.LogWarning("Country delete failed because government was not found. CountryId: {CountryId}", request.Id);
                return BaseResult.UnExpected(Errors.FailedOperation("An error occurred while removing the country."));
            }
            await unitOfWork.SaveChangesAsync(ct);
            logger.LogInformation("Country deleted successfully. Id: {CountryId}", request.Id);
            return BaseResult.Success();
        }
    }
}
