using TransitNova.BusinessLayer.DTOs.Vehicle;
using TransitNova.Domain.Enums.Carrier;
namespace TransitNovaUI.BusinessLayer.DTOs.Vehicle;

public sealed class UiCreateVehicleDto
{
    public VehicleType VehicleType { get; set; }
    public string PlateNumber { get; set; } = string.Empty;
    public decimal CapacityWeight { get; set; }
    public decimal CapacityVolume { get; set; }
    public bool IsRefrigerated { get; set; }
    public Guid CarrierId { get; set; }

    public static CreateVehicleDto ToDto(UiCreateVehicleDto source) =>
        new()
        {
            VehicleType = source.VehicleType,
            PlateNumber = source.PlateNumber,
            CapacityWeight = source.CapacityWeight,
            CapacityVolume = source.CapacityVolume,
            IsRefrigerated = source.IsRefrigerated,
            CarrierId = source.CarrierId
        };
}
