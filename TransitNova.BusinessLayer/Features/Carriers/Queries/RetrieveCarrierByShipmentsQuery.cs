
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Shipment;
namespace TransitNova.BusinessLayer.Features.Carriers.Queries
{
    public sealed record RetrieveCarrierByShipmentsQuery(Guid CarrierId, Guid CurrentUser)
        : IQuery<Result<IEnumerable<RetrieveShipmentDto>>>;

}
