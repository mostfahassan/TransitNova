using AutoMapper;
using TransitNova.BusinessLayer.DTOs.ZoneDtos;
using TransitNova.Domain.Entities.MainEntities;

namespace TransitNova.BusinessLayer.Common.Mappings.LocationMapping
{
    public class ZoneMappingProfile : Profile
    {
        public ZoneMappingProfile()
        {
            CreateMap<Zone, ZoneDto>()
                .ForMember(d => d.CityName, opt => opt.MapFrom(s => s.City != null ? s.City.Name : string.Empty))
                .ForMember(d => d.CountryId, opt => opt.MapFrom(s => s.City != null ? s.City.GovernmentId : default))
                .ForMember(d => d.CountryName, opt => opt.MapFrom(s => s.City != null && s.City.Government != null ? s.City.Government.Country.Name : string.Empty));
        }
    }
}
