
namespace TransitNova.BusinessLayer.DTOs.Carrier
{
    public sealed record RatingCarrierDto(Guid CarrierId, string? Comment, int Rating);
}
