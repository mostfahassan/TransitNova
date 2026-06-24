using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Country;
using TransitNova.BusinessLayer.Features.Location.Governments.Commands;
using TransitNova.BusinessLayer.Interfaces.Repositories.GenericRepository;
using TransitNova.BusinessLayer.Interfaces.Services.UnitOfWork;
using TransitNova.Domain.Entities.MainEntities;

namespace TransitNova.BusinessLayer.Features.Location.Governments.Handlers.ApplyCommands
{
    public sealed class CreateGovernmentHandler(
        IGenericRepository<Government, int> governmentRepository,
        IGenericRepository<Country, int> countryRepository,
        IUnitOfWork unitOfWork,
        ILogger<CreateGovernmentHandler> logger)
        : ICommandHandler<CreateGovernmentCommand, Result<GovernmentDto>>
    {
        public async Task<Result<GovernmentDto>> Handle(CreateGovernmentCommand request, CancellationToken ct)
        {
            var countryExists = await countryRepository.ExistsAsync(request.CountryId, ct);
            if (!countryExists)
            {
                logger.LogWarning("Government creation failed because country was not found. CountryId: {CountryId}", request.CountryId);
                return Result<GovernmentDto>.NotFound(Errors.NotFound("Country not found."));
            }

            var government = Government.Create(request.Name.Trim(), request.CountryId);
            await governmentRepository.AddAsync(government, ct);
            await unitOfWork.SaveChangesAsync(ct);

            logger.LogInformation("Government created successfully. GovernmentId: {GovernmentId}, CountryId: {CountryId}", government.Id, request.CountryId);

            return Result<GovernmentDto>.Success(new GovernmentDto
            {
                Id = government.Id,
                Name = government.Name
            });
        }
    }
}
