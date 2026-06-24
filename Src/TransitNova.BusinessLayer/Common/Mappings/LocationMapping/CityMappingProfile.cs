using AutoMapper;
using TransitNova.BusinessLayer.DTOs.City;
using TransitNova.Domain.Entities.MainEntities;

namespace TransitNova.BusinessLayer.Common.Mappings.LocationMapping
{
    public class CityMappingProfile : Profile
    {
        public CityMappingProfile()
        {
            CreateMap<City, CityDto>()
                .ForMember(d => d.GovernmentName, opt => opt.MapFrom(s => s.Government != null ? s.Government.Name : string.Empty));
        }
    }
}
