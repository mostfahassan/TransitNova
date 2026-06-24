using TransitNova.BusinessLayer.DTOs.Carrier;
using TransitNova.Domain.Enums.Carrier;
namespace TransitNovaUI.BusinessLayer.DTOs.Carrier;

public sealed record UiAssignCarrierDto(Guid CarrierId)
{
    public static AssignCarrierDto ToDto(UiAssignCarrierDto source) => new(source.CarrierId);

}
