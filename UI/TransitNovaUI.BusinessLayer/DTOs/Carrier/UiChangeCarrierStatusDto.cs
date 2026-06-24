using TransitNova.BusinessLayer.DTOs.Carrier;
using TransitNova.Domain.Enums.Carrier;
namespace TransitNovaUI.BusinessLayer.DTOs.Carrier;

public sealed record UiChangeCarrierStatusDto(CarrierStatus Status)
{
    public static ChangeCarrierStatus ToDto(UiChangeCarrierStatusDto source) =>
        new(source.Status);

}
