using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Carrier;
using TransitNova.Domain.Enums.Carrier;
using TransitNova.Domain.Enums.Users;
using TransitNovaUI.BusinessLayer.Common.ResultPattern;
namespace TransitNovaUI.BusinessLayer.DTOs.Carrier;

public sealed class UiCarrierVehicleDto
{
    public Guid Id { get; set; }
    public VehicleType VehicleType { get; set; }
    public string PlateNumber { get; set; } = string.Empty;
    public decimal CapacityWeight { get; set; }
    public decimal CapacityVolume { get; set; }
    public bool IsRefrigerated { get; set; }
    public bool IsActive { get; set; }

    public static UiCarrierVehicleDto ToUiDto(CarrierVehicleDto source) =>
        new()
        {
            Id = source.Id,
            VehicleType = source.VehicleType,
            PlateNumber = source.PlateNumber,
            CapacityWeight = source.CapacityWeight,
            CapacityVolume = source.CapacityVolume,
            IsRefrigerated = source.IsRefrigerated,
            IsActive = source.IsActive
        };
}
