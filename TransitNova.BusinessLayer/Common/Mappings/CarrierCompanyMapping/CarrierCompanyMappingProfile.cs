using AutoMapper;
using TransitNova.BusinessLayer.DTOs.CarrierCompany;
using TransitNova.Domain.Entities.MainEntities;

namespace TransitNova.BusinessLayer.Common.Mappings.CarrierCompanyMapping
{
    public class CarrierCompanyMappingProfile : Profile
    {
        public CarrierCompanyMappingProfile()
        {
            CreateMap<CarrierCompany, RetrieveCarrierCompany>()
                .ForMember(dest => dest.PhoneNumber,
                    opt => opt.MapFrom(src => src.Phone))
                .ForMember(dest => dest.CityName,
                    opt => opt.MapFrom(src => src.Zone != null && src.Zone.City != null
                        ? src.Zone.City.Name
                        : string.Empty))
                .ForMember(dest => dest.CountryName,
                    opt => opt.MapFrom(src => src.Zone != null && src.Zone.City != null && src.Zone.City.Government != null
                        ? src.Zone.City.Government.Country.Name
                        : string.Empty));
        }
    }
}
