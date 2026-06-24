using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Shipment;
using TransitNova.Domain.Enums.Shipment;
namespace TransitNovaUI.BusinessLayer.DTOs.Shipment;

public sealed class UiRetrieveShipmentDto
{
    public Guid Id { get; set; }
    public Guid ReceiverId { get; set; }
    public Guid SenderId { get; set; }
    public UiUserSummaryDto Receiver { get; set; } = new();
    public UiUserSummaryDto? Sender { get; set; }
    public string DeliveryAddress { get; set; } = string.Empty;
    public string PickupAddress { get; set; } = string.Empty;
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
    public Guid? PackageBundleId { get; set; }
    public DateTime CreatedAt { get; set; }

    public static UiRetrieveShipmentDto ToUiDto(RetrieveShipmentDto source) =>
        new()
        {
            Id = source.Id,
            ReceiverId = source.ReceiverId,
            SenderId = source.SenderId,
            Receiver = UiUserSummaryDto.ToUiDto(source.Receiver),
            Sender = source.Sender is null ? null : UiUserSummaryDto.ToUiDto(source.Sender),
            DeliveryAddress = source.DeliveryAddress,
            PickupAddress = source.PickupAddress,
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
            PackageBundleId = source.PackageBundleId,
            CreatedAt = source.CreatedAt
        };

    public static UiPagedResult<UiRetrieveShipmentDto> ToUiPagedDto(PagedResult<RetrieveShipmentDto> source) =>
        UiPagedResult<UiRetrieveShipmentDto>.From(
            source.Data.Select(ToUiDto),
            source.TotalCount,
            source.PageNumber,
            source.PageSize);
}
