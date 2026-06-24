using AutoMapper;
using TransitNova.BusinessLayer.Common.CommonData;
using TransitNova.BusinessLayer.DTOs.UserProfile;
using TransitNova.BusinessLayer.DTOs.UserProfile.Admin;
using TransitNova.BusinessLayer.DTOs.UserProfile.OperationManager;
using TransitNova.Domain.Entities.Common;
using TransitNova.Domain.Entities.MainEntities;
namespace TransitNova.BusinessLayer.Common.Mappings.UserProfileMapping
{
    public class UserProfileMappingProfile : Profile
    {
        public UserProfileMappingProfile()
        {
            CreateMap<BaseInfo<Guid>, CommonRetrieveData>()
                    .ForMember(dest => dest.FullName,
                        opt => opt.MapFrom(src => src.FirstName + " " + src.LastName))
                    .ForMember(dest => dest.CityName,
                      opt => opt.MapFrom(src => src.City != null
                          ? src.City.Name
                    : string.Empty))
                    .ForMember(dest => dest.GovernmentName,
                        opt => opt.MapFrom(src => src.City != null && src.City.Government != null
                            ? src.City.Government.Name
                            : string.Empty))
                    .ForMember(dest => dest.CountryName,
                        opt => opt.MapFrom(src => src.City != null &&
                                                  src.City.Government != null &&
                                                  src.City.Government.Country != null
                            ? src.City.Government.Country.Name
                            : string.Empty));

            CreateMap<BaseInfo<Guid>, UserSummaryDto>()
                .ForMember(dest => dest.FullName,
                    opt => opt.MapFrom(src => src.FirstName + " " + src.LastName));

            // Summary CreateMap<UserProfile, UserSummaryDto>();

            CreateMap<UserProfile, UserSummaryDto>()
                .IncludeBase<BaseInfo<Guid>, UserSummaryDto>();

            CreateMap<ReceiverProfile, UserSummaryDto>()
                .IncludeBase<BaseInfo<Guid>, UserSummaryDto>();

            CreateMap<Carrier, UserSummaryDto>()
                .IncludeBase<BaseInfo<Guid>, UserSummaryDto>();

            CreateMap<OperationManagerProfile, UserSummaryDto>()
                .IncludeBase<BaseInfo<Guid>, UserSummaryDto>();

            CreateMap<AdminProfile, UserSummaryDto>()
              .IncludeBase<BaseInfo<Guid>, UserSummaryDto>();



            CreateMap<UserProfile, UserProfileDto>()
                .IncludeBase<BaseInfo<Guid>, CommonRetrieveData>()
                .ForMember(dest => dest.TotalShipmentsSent,
                    opt => opt.MapFrom(src => src.SentShipments == null ? 0 : src.SentShipments.Count))
                .ForMember(dest => dest.BundleName,
                    opt => opt.MapFrom(src => src.Subscriptions
                        .Where(subscription => subscription.IsActive)
                        .Select(subscription => subscription.Bundle.BundleName)
                        .FirstOrDefault()));

           
            CreateMap<AdminProfile, AdminProfileDto>()
                .IncludeBase<BaseInfo<Guid>, CommonRetrieveData>();
             
        }
    }
}
