using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Shipment;
using TransitNova.BusinessLayer.Features.UserOperations.Queries;
using TransitNova.BusinessLayer.Interfaces.Repositories.UserRepository;
namespace TransitNova.BusinessLayer.Features.UserOperations.Handlers.QueriesHandler
{
    public sealed class GetUserShipmentHandler(
        IUserQueryRepository userQueryRepository,
        ILogger<GetUserShipmentHandler> logger)
        : IQueryHandler<GetUserShipmentQuery, Result<RetrieveShipmentDto>>
    {
        public async Task<Result<RetrieveShipmentDto>> Handle(GetUserShipmentQuery request, CancellationToken ct)
        {
            logger.LogInformation("Retrieving shipment {ReferecneId} for User {UserId}", request.ShipmentId, request.AppUserId);

            var shipment = await userQueryRepository.GetUserShipmentDetailsAsync(request.AppUserId, request.ShipmentId, ct);
            if (shipment is null)
            {
                logger.LogWarning("Shipment {ReferecneId} was not found for User {UserId}", request.ShipmentId, request.AppUserId);
                var notFoundResult = Result<RetrieveShipmentDto>.NotFound(Errors.ShipmentNotFound("Shipment not found."));
                return notFoundResult;
            }

            logger.LogInformation("Shipment {ReferecneId} retrieved successfully for User {UserId}", request.ShipmentId, request.AppUserId);
            var result = Result<RetrieveShipmentDto>.Success(shipment);
            return result;
        }
    }
}
