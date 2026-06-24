using ApiRefreshToken = TransitNova.BusinessLayer.DTOs.RefreshToken.RefreshToken;

namespace TransitNovaUI.BusinessLayer.DTOs.RefreshToken;

public sealed record UiRefreshTokenDto(string Token)
{
    public static ApiRefreshToken ToDto(UiRefreshTokenDto source) => new(source.Token);

}
