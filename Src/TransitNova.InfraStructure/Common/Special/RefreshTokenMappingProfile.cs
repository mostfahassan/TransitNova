using AutoMapper;
using TransitNova.BusinessLayer.DTOs.RefreshToken;
namespace TransitNova.InfraStructure.Common.Special
{
    public class RefreshTokenMappingProfile : Profile
    {
        public RefreshTokenMappingProfile()
        {
            CreateMap<RefreshToken, RefreshTokenDto>()
                .ForMember(dest => dest.ExpiresOn,
                    opt => opt.MapFrom(src => src.ExpiresAt));

            CreateMap<RefreshTokenDto, RefreshToken>(MemberList.None)
                .ForMember(dest => dest.ExpiresAt,
                    opt => opt.MapFrom(src => src.ExpiresOn))
                .ForMember(dest => dest.User,
                    opt => opt.Ignore());
        }
    }
}
