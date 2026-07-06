using AutoMapper;
using TransitNova.BusinessLayer.DTOs.AppUser;
using TransitNova.BusinessLayer.DTOs.UserProfile.Auth;
namespace TransitNova.InfraStructure.Common.Special
{
    public class UserMapping : Profile
    {
        public UserMapping()
        {
            // RegisterDto → AppUserDto
            CreateMap<RegisterDto, AppUserDto>()
                .ForMember(dest => dest.Id,
                    opt => opt.Ignore())
                .ForMember(dest => dest.EmailConfirmed,
                    opt => opt.Ignore())
                .ForMember(dest => dest.IsLockedOut,
                    opt => opt.Ignore())
                .ForMember(dest => dest.Roles,
                    opt => opt.Ignore())

                .ForMember(dest => dest.Email,
                    opt => opt.MapFrom(src => src.Email))

                .ForMember(dest => dest.UserName,
                    opt => opt.MapFrom(src => src.UserName))

                .ForMember(dest => dest.PhoneNumber,
                    opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.FullName,
                    opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}".Trim()));


            CreateMap<AppUserDto, AppUser>(MemberList.None)
                    .ForMember(dest => dest.Id,
                        opt => opt.MapFrom(src => src.Id))
                    .ForMember(dest => dest.UserName,
                        opt => opt.MapFrom(src => src.UserName))
                    .ForMember(dest => dest.Email,
                        opt => opt.MapFrom(src => src.Email))
                    .ForMember(dest => dest.PhoneNumber,
                        opt => opt.MapFrom(src => src.PhoneNumber))
                    .ForMember(dest => dest.FullName,
                        opt => opt.MapFrom(src => src.FullName))
                    .ForMember(dest => dest.EmailConfirmed,
                        opt => opt.MapFrom(src => src.EmailConfirmed));

            CreateMap<AppUser, AppUserDto>()
                    .ForMember(dest => dest.Id,
                        opt => opt.MapFrom(src => src.Id))
                    .ForMember(dest => dest.UserName,
                        opt => opt.MapFrom(src => src.UserName))
                    .ForMember(dest => dest.Email,
                        opt => opt.MapFrom(src => src.Email))
                    .ForMember(dest => dest.PhoneNumber,
                        opt => opt.MapFrom(src => src.PhoneNumber))
                    .ForMember(dest => dest.FullName,
                        opt => opt.MapFrom(src => src.FullName))
                    .ForMember(dest => dest.UserType,
                        opt => opt.MapFrom(src => src.UserType))
                    .ForMember(dest => dest.EmailConfirmed,
                        opt => opt.MapFrom(src => src.EmailConfirmed))
                    .ForMember(dest => dest.IsLockedOut,
                        opt => opt.MapFrom(src => src.LockoutEnd.HasValue))
                    .ForMember(dest => dest.Roles,
                        opt => opt.Ignore());
            }
        }
    }


