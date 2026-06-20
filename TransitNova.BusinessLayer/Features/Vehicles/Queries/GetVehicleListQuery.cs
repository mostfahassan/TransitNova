using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Vehicle;

namespace TransitNova.BusinessLayer.Features.Vehicles.Queries
{
    public sealed record GetVehicleListQuery()
        : IQuery<Result<List<VehicleDto>>>;
}
