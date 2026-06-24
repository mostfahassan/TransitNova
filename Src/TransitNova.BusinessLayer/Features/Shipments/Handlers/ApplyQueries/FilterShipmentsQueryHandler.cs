
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Shipment;
using TransitNova.BusinessLayer.Features.Shipments.Queries;
using TransitNova.BusinessLayer.Interfaces.Repositories.ShipmentRepository;
namespace TransitNova.BusinessLayer.Features.Shipments.Handlers.ApplyQueries
{
    public class FilterShipmentsQueryHandler
        (IShipmentQueryRepository shipmentRepo)
        : IQueryHandler<FilterShipmentsQuery, Result<PagedResult<RetrieveShipmentDto>>>
    {
        public async Task<Result<PagedResult<RetrieveShipmentDto>>> Handle(FilterShipmentsQuery request, CancellationToken cancellationToken)
        {
            var filteredShipments = await shipmentRepo.FilterAsync(request.FilterCriteria, cancellationToken);
           
            if (!filteredShipments.Data.Any()) return Result<PagedResult<RetrieveShipmentDto>>.Success(filteredShipments);

            return Result<PagedResult<RetrieveShipmentDto>>.Success(filteredShipments);
        }
    }
}
