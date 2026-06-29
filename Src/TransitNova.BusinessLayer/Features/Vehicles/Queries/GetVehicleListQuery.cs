using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Vehicle;
using TransitNova.BusinessLayer.Interfaces.MarkerInterfaces;
using TransitNova.Domain.Contracts.Caching;
namespace TransitNova.BusinessLayer.Features.Vehicles.Queries
{
    public sealed record GetVehicleListQuery()
        : IQuery<Result<List<VehicleDto>>>, ICachable
    {
        public string CacheKey => CacheKeys.Vehicles.List;
    }

}

