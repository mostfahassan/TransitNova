using AutoMapper;
using TransitNova.BusinessLayer.Common.CommonData;
using TransitNova.BusinessLayer.DTOs.Carrier;
using TransitNova.Domain.Entities.Common;
using TransitNova.Domain.Entities.MainEntities;
using TransitNova.Domain.Enums.Trip;
namespace TransitNova.BusinessLayer.Common.Mappings.CarrierMapping
{
    public class CarrierMappingProfile : Profile
    {
        public CarrierMappingProfile()
        {
            CreateMap<Vehicle, CarrierVehicleDto>();

            CreateMap<Carrier, CarrierProfileDto>()
                .IncludeBase<BaseInfo<Guid>, CommonRetrieveData>()
                .ForMember(dest => dest.Experience,
                    opt => opt.MapFrom(src => src.YearsOfExperience))
                .ForMember(dest => dest.Rating,
                    opt => opt.MapFrom(src => src.AverageRating));

            CreateMap<Carrier, CarrierSummaryDetailsDto>()
               .ForMember(dest => dest.ActiveTripsCount,
                   opt => opt.MapFrom(src =>
                       src.Trips.Count(t =>
                           t.Status == TripStatus.Active ||
                           t.Status == TripStatus.Planned)))
               .ForMember(dest => dest.Rating,
                   opt => opt.MapFrom(src => src.AverageRating))

               .ForMember(dest => dest.ServedCities,
                   opt => opt.MapFrom(src =>
                       src.ServedZones
                           .Select(sz => sz.City.Name)
                           .Distinct()));
        }
    }
}
