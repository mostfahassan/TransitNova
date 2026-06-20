using TransitNova.BusinessLayer.DTOs.Vehicle;
using TransitNova.Domain.Enums.Carrier;

namespace TransitNovaUI.BusinessLayer.DTOs.Vehicle;

public sealed class UiCreateVehicleDto
{
    public Guid Id { get; set; }
    public VehicleType VehicleType { get; set; }
    public string PlateNumber { get; set; } = string.Empty;
    public decimal CapacityWeight { get; set; }
    public decimal CapacityVolume { get; set; }
    public bool IsRefrigerated { get; set; }
    public bool IsActive { get; set; }
    public Guid CarrierId { get; set; }
    public string CarrierName { get; set; } = string.Empty;
    public string CarrierCode { get; set; } = string.Empty;
    public decimal CarrierRating { get; set; }
    public CarrierStatus CarrierStatus { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public static VehicleDto ToDto(UiCreateVehicleDto source) =>
        new()
        {
            Id = source.Id,
            VehicleType = source.VehicleType,
            PlateNumber = source.PlateNumber,
            CapacityWeight = source.CapacityWeight,
            CapacityVolume = source.CapacityVolume,
            IsRefrigerated = source.IsRefrigerated,
            IsActive = source.IsActive,
            CarrierId = source.CarrierId,
            CarrierName = source.CarrierName,
            CarrierCode = source.CarrierCode,
            CarrierRating = source.CarrierRating,
            CarrierStatus = source.CarrierStatus,
            CreatedAt = source.CreatedAt,
            UpdatedAt = source.UpdatedAt
        };
}

public sealed class UiVehicleDto
{
    public Guid Id { get; set; }
    public VehicleType VehicleType { get; set; }
    public string PlateNumber { get; set; } = string.Empty;
    public decimal CapacityWeight { get; set; }
    public decimal CapacityVolume { get; set; }
    public bool IsRefrigerated { get; set; }
    public bool IsActive { get; set; }
    public Guid CarrierId { get; set; }
    public string CarrierName { get; set; } = string.Empty;
    public string CarrierCode { get; set; } = string.Empty;
    public decimal CarrierRating { get; set; }
    public CarrierStatus CarrierStatus { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public static UiVehicleDto ToUiDto(VehicleDto source) =>
        new()
        {
            Id = source.Id,
            VehicleType = source.VehicleType,
            PlateNumber = source.PlateNumber,
            CapacityWeight = source.CapacityWeight,
            CapacityVolume = source.CapacityVolume,
            IsRefrigerated = source.IsRefrigerated,
            IsActive = source.IsActive,
            CarrierId = source.CarrierId,
            CarrierName = source.CarrierName,
            CarrierCode = source.CarrierCode,
            CarrierRating = source.CarrierRating,
            CarrierStatus = source.CarrierStatus,
            CreatedAt = source.CreatedAt,
            UpdatedAt = source.UpdatedAt
        };
}
