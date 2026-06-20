using AutoMapper;
using TransitNova.BusinessLayer.DTOs.Vehicle;
using TransitNova.Domain.Entities.MainEntities;

namespace TransitNova.BusinessLayer.Common.Mappings.VehicleMapping
{
    public class VehicleMappingProfile : Profile
    {
        public VehicleMappingProfile()
        {
            CreateMap<Vehicle, VehicleDto>()
                .ForMember(dest => dest.CarrierName,
                    opt => opt.MapFrom(src => src.Carrier != null ? src.Carrier.FirstName + " " + src.Carrier.LastName : string.Empty))
                .ForMember(dest => dest.CarrierCode,
                    opt => opt.MapFrom(src => src.Carrier != null ? src.Carrier.Code : string.Empty))
                .ForMember(dest => dest.CarrierRating,
                    opt => opt.MapFrom(src => src.Carrier != null ? src.Carrier.AverageRating : default))
                .ForMember(dest => dest.CarrierStatus,
                    opt => opt.MapFrom(src => src.Carrier != null ? src.Carrier.Status : default));
        }
    }
}
