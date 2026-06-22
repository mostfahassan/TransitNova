using AutoMapper;
using TransitNova.BusinessLayer.DTOs.Vehicle;
using TransitNova.Domain.Entities.MainEntities;

namespace TransitNova.BusinessLayer.Common.Mappings.VehicleMapping
{
    public class VehicleMappingProfile : Profile
    {
        public VehicleMappingProfile()
        {
            CreateMap<CreateVehicleDto, Vehicle>(MemberList.None)
                .ConstructUsing(source => Vehicle.Create(
                    source.VehicleType,
                    source.PlateNumber,
                    source.CapacityWeight,
                    source.CapacityVolume,
                    source.IsRefrigerated,
                    source.CarrierId));

            CreateMap<Vehicle, VehicleDto>()
                .ForMember(destination => destination.CarrierName,
                    options => options.MapFrom(source => source.Carrier.FullName))
                .ForMember(destination => destination.CarrierCode,
                    options => options.MapFrom(source => source.Carrier.Code))
                .ForMember(destination => destination.CarrierRating,
                    options => options.MapFrom(source => source.Carrier.AverageRating))
                .ForMember(destination => destination.CarrierStatus,
                    options => options.MapFrom(source => source.Carrier.Status));
        }
    }
}
