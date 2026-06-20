
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.Domain.Enums.Shipment;

namespace TransitNova.BusinessLayer.Features.Shipments.Queries
{
    public sealed record GetShipmentStatisticsQuery() : IQuery<Result<Dictionary<ShipmentStatuses, int>>>;
    
}
