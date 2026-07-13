using AutoMapper;
using TransitNova.BusinessLayer.DTOs.Country;
using TransitNova.Domain.Entities.MainEntities;

namespace TransitNova.BusinessLayer.Common.Mappings.LocationMapping
{
    public class CountryMappingProfile : Profile
    {
        public CountryMappingProfile()
        {
            CreateMap<Country, CountryDto>();
            CreateMap<Government, GovernmentDto>()
                .ForMember(dest => dest.CountryName,
                    opt => opt.MapFrom(src => src.Country != null ? src.Country.Name : string.Empty));
        }
    }
}
