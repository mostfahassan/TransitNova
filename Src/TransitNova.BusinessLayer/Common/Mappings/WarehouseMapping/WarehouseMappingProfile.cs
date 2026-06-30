using AutoMapper;
using TransitNova.BusinessLayer.DTOs.Warehouse;
using TransitNova.Domain.Entities.MainEntities;
using TransitNova.Domain.Enums.Trip;
namespace TransitNova.BusinessLayer.Common.Mappings.WarehouseMapping
{
    public class WarehouseMappingProfile : Profile
    {
        public WarehouseMappingProfile()
        {
            CreateMap<Warehouse, WarehouseDto>()
                .ForMember(dest => dest.ZoneIds,
                    opt => opt.MapFrom(src => src.ZonesServed.Select(z => z.Id).ToList()))
                .ForMember(dest => dest.ZoneNames,
                    opt => opt.MapFrom(src => src.ZonesServed.Select(z => z.Name).ToList()))
                .ForMember(dest => dest.CarrierCount,
                    opt => opt.MapFrom(src => src.Carriers.Count))
                .ForMember(dest => dest.ActiveTripsCount,
                    opt => opt.MapFrom(src => src.Trips.Count(t => t.Status == TripStatus.Active)))
                .ForMember(dest => dest.WarehouseManagerName,
                    opt => opt.MapFrom(src => src.Manager.FullName));
        }
    }
}
