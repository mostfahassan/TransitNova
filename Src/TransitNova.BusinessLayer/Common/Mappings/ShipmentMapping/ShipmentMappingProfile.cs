using AutoMapper;
using TransitNova.BusinessLayer.DTOs.Shipment;
using TransitNova.Domain.Entities.Common;
using TransitNova.Domain.Entities.MainEntities;

namespace TransitNova.BusinessLayer.Common.Mappings.ShipmentMapping
{
    public class ShipmentMappingProfile : Profile
    {
        public ShipmentMappingProfile()
        {
            CreateMap<PackageSpecification, PackageSpecificationDto>();
            CreateMap<PackageSpecificationDto, PackageSpecification>()
            .ConstructUsing(src => new PackageSpecification(src.Weight, src.Width, src.Height, src.Length));
            CreateMap<Shipment, RetrieveShipmentDto>()
                 .ForMember(dest => dest.ShippingCost,
                 opt => opt.MapFrom(src => src.ShipmentCost))
                 .ForMember(dest =>dest.RejectionReason,
                     opt => opt.MapFrom(src => src.RejectionReason != null ? src.RejectionReason : null))
                 .ForMember(dest => dest.TransportationMode,
                     opt => opt.MapFrom(src => src.Mode))

                 .ForMember(dest => dest.TrackingNumber,
                     opt => opt.MapFrom(src => src.TrackingNumber))

                 .ForMember(dest => dest.EstimatedDeliveryDate,
                     opt => opt.MapFrom(src => src.EstimatedDeliveryDate))

                 .ForMember(dest => dest.ShipmentStates,
                     opt => opt.MapFrom(src => src.ShipmentStates));
        }
    }
}
