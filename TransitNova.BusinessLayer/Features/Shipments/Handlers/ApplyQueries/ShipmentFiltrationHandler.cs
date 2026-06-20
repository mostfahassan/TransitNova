
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Shipment;
using TransitNova.BusinessLayer.Features.Shipments.Commands;
using TransitNova.BusinessLayer.Interfaces.Repositories.ShipmentRepository;
namespace TransitNova.BusinessLayer.Features.Shipments.Handlers.ApplyQueries
{
    public class ShipmentFiltrationHandler
        (IShipmentQueryRepository shipmentRepo)
        : ICommandHandler<ShipmentFiltrationCommand, Result<PagedResult<RetrieveShipmentDto>>>
    {
        public async Task<Result<PagedResult<RetrieveShipmentDto>>> Handle(ShipmentFiltrationCommand request, CancellationToken cancellationToken)
        {
            var filteredShipments = await shipmentRepo.FilterAsync(request.FilterCriteria, cancellationToken);
           
            if (!filteredShipments.Data.Any()) return Result<PagedResult<RetrieveShipmentDto>>.Success(filteredShipments);

            return Result<PagedResult<RetrieveShipmentDto>>.Success(filteredShipments);
        }
    }
}
