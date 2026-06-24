using AutoMapper;
using TransitNova.BusinessLayer.DTOs.ShipmentStatusDto;
using TransitNova.Domain.Entities.MainEntities;

namespace TransitNova.BusinessLayer.Common.Mappings.ShipmentStatusMapping
{
    public class ShipmentStatusMappingProfile : Profile
    {
        public ShipmentStatusMappingProfile()
        {
            CreateMap<ShipmentStatus, RetrieveShipmentStatusDto>()
                .ForMember(dest => dest.StatusType,
                    opt => opt.MapFrom(src => src.StatusType))
                .ForMember(dest => dest.ChangedAt,
                    opt => opt.MapFrom(src => src.UpdatedAt ?? src.CreatedAt))
                .ForMember(dest => dest.Carrier,
                    opt => opt.MapFrom(src => src.Carrier));
               
        }
    }
}
