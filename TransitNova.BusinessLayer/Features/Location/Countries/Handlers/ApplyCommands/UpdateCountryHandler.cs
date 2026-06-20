using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.Features.Location.Countries.Commands;
using TransitNova.BusinessLayer.Interfaces.Repositories.LocationRepository;
using TransitNova.BusinessLayer.Interfaces.Services.UnitOfWork;
using TransitNova.Domain.DomainExceptions;
using TransitNova.Domain.Entities.MainEntities;

namespace TransitNova.BusinessLayer.Features.Location.Countries.Handlers.ApplyCommands
{
    public sealed class UpdateCountryHandler(
        ICountryRepository repository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateCountryHandler> logger)
        : ICommandHandler<UpdateCountryCommand, BaseResult>
    {
        public async Task<BaseResult> Handle(UpdateCountryCommand request, CancellationToken ct)
        {
            var entity = await repository.GetByIdAsync<Country>(request.Dto.CountryId, ct);
            if (entity == null)
            {
                logger.LogInformation("No City Found With Id {CountryId}", request.Dto.CountryId);
                throw new EntityNotFoundException($"No Country With ID:{request.Dto.CountryId} Has Been Founded", "COUNTRY_NOT_FOUNDED", nameof(Country));
            }
            entity.Update(request.Dto.Name.Trim());
            repository.Update(entity);
            await unitOfWork.SaveChangesAsync(ct);
            logger.LogInformation("Country updated successfully. Id: {CountryId}", request.Dto.CountryId);
            return BaseResult.Success();
        }
    }
}
