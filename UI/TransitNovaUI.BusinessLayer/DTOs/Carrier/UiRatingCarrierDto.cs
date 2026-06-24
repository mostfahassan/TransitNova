using TransitNova.BusinessLayer.DTOs.Carrier;
using TransitNova.Domain.Enums.Carrier;
namespace TransitNovaUI.BusinessLayer.DTOs.Carrier;

public sealed record UiRatingCarrierDto(Guid CarrierId, string? Comment, int Rating)
{
    public static RatingCarrierDto ToDto(UiRatingCarrierDto source) =>
        new(source.CarrierId, source.Comment, source.Rating);

}
