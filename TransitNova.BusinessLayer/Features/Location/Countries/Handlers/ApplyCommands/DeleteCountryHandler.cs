using TransitNova.BusinessLayer.Common.CQRS;
using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.Features.Location.Countries.Commands;
using TransitNova.BusinessLayer.Interfaces.Repositories.LocationRepository;
using TransitNova.BusinessLayer.Interfaces.Services.UnitOfWork;

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
            await repository.DeleteAsync(request.Id, ct);

            await unitOfWork.SaveChangesAsync(ct);
            logger.LogInformation("Country deleted successfully. Id: {CountryId}", request.Id);
            return BaseResult.Success();
        }
    }
}
