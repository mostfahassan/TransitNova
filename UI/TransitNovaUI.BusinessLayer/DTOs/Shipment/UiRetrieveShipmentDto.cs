using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Shipment;
using TransitNova.Domain.Enums.Shipment;
using TransitNovaUI.BusinessLayer.Common.CommonData;
namespace TransitNovaUI.BusinessLayer.DTOs.Shipment;

public sealed class UiRetrieveShipmentDto
{
    public Guid Id { get; set; }
    public Guid ReceiverId { get; set; }
    public Guid SenderId { get; set; }
    public UiUserSummaryDto Receiver { get; set; } = new();
    public UiUserSummaryDto? Sender { get; set; }
    public UiAddressDto DeliveryAddress { get; set; } = new();
    public UiAddressDto PickupAddress { get; set; } = new();
    public string? RejectionReason { get; set; }
    public UiPackageSpecificationDto PackageSpecification { get; set; } = new();
    public Currency Currency { get; set; }
    public TransportationMode TransportationMode { get; set; }
    public ShipmentStatuses CurrentStatus { get; set; }
    public decimal ShippingCost { get; set; }
    public DateTime? EstimatedDeliveryDate { get; set; }
    public string TrackingNumber { get; set; } = string.Empty;
    public List<UiRetrieveShipmentStatusDto> ShipmentStates { get; set; } = [];
    public enShipmentType ShipmentType { get; set; }
    public DateTime CreatedAt { get; set; }

    public static UiRetrieveShipmentDto ToUiDto(RetrieveShipmentDto source) =>
        new()
        {
            Id = source.Id,
            ReceiverId = source.ReceiverId,
            SenderId = source.SenderId,
            Receiver = UiUserSummaryDto.ToUiDto(source.Receiver),
            Sender = source.Sender is null ? null : UiUserSummaryDto.ToUiDto(source.Sender),
            DeliveryAddress = UiAddressDto.ToUiDto(source.DeliveryAddress),
            PickupAddress = UiAddressDto.ToUiDto(source.PickupAddress),
            RejectionReason = source.RejectionReason,
            PackageSpecification = UiPackageSpecificationDto.ToUiDto(source.PackageSpecification),
            Currency = source.Currency,
            TransportationMode = source.TransportationMode,
            CurrentStatus = source.CurrentStatus,
            ShippingCost = source.ShippingCost,
            EstimatedDeliveryDate = source.EstimatedDeliveryDate,
            TrackingNumber = source.TrackingNumber,
            ShipmentStates =[ ..source.ShipmentStates.Select(UiRetrieveShipmentStatusDto.ToUiDto)],
            ShipmentType = source.ShipmentType,
            CreatedAt = source.CreatedAt
        };

    public static UiPagedResult<UiRetrieveShipmentDto> ToUiPagedDto(PagedResult<RetrieveShipmentDto> source) =>
        UiPagedResult<UiRetrieveShipmentDto>.From(
            source.Data.Select(ToUiDto),
            source.TotalCount,
            source.PageNumber,
            source.PageSize);
}
