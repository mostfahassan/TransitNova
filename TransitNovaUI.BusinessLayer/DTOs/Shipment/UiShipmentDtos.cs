using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Shipment;
using TransitNova.Domain.Enums.Shipment;
using TransitNovaUI.BusinessLayer.Common.CommonData;
using TransitNovaUI.BusinessLayer.DTOs.Payment;
namespace TransitNovaUI.BusinessLayer.DTOs.Shipment;
public sealed class UiPackageSpecificationDto
{
    public decimal Weight { get; set; }
    public decimal Width { get; set; }
    public decimal Height { get; set; }
    public decimal Length { get; set; }

    public static UiPackageSpecificationDto ToUiDto(PackageSpecificationDto source) =>
        new()
        {
            Weight = source.Weight,
            Width = source.Width,
            Height = source.Height,
            Length = source.Length
        };
}

public sealed class UiPackageSpecificationRequestDto
{
    public decimal Weight { get; set; }
    public decimal Width { get; set; }
    public decimal Height { get; set; }
    public decimal Length { get; set; }

    public static PackageSpecificationDto ToDto(UiPackageSpecificationRequestDto source) =>
        new()
        {
            Weight = source.Weight,
            Width = source.Width,
            Height = source.Height,
            Length = source.Length
        };
}

public sealed record UiCreateShipmentDto(
    UiCreateReceiverDto Receiver,
    UiPackageSpecificationRequestDto PackageSpecification,
    Currency Currency,
    DateTime? PickUpDate,
    TransportationMode TransportationMode,
    enShipmentType ShipmentDeliveryType,
    string DeliveryAddress,
    string PickupAddress,
    Guid? PackageBundleId)
{
    public static CreateShipmentDto ToDto(UiCreateShipmentDto source) =>
        new(
            UiCreateReceiverDto.ToDto(source.Receiver),
            UiPackageSpecificationRequestDto.ToDto(source.PackageSpecification),
            source.Currency,
            source.PickUpDate,
            source.TransportationMode,
            source.ShipmentDeliveryType,
            source.DeliveryAddress,
            source.PickupAddress,
            source.PackageBundleId);

}

public sealed record UiUpdateShipmentDto(
    Guid Id,
    Guid? ReceiverId,
    string? DeliveryAddress,
    string? PickupAddress,
    UiPackageSpecificationRequestDto? PackageSpecification,
    enShipmentType? ShipmentType,
    TransportationMode? TransportationMode)
{
    public static UpdateShipmentDto ToDto(UiUpdateShipmentDto source) =>
        new(
            source.Id,
            source.ReceiverId,
            source.DeliveryAddress,
            source.PickupAddress,
            source.PackageSpecification is null
                ? null
                : UiPackageSpecificationRequestDto.ToDto(source.PackageSpecification),
            source.ShipmentType,
            source.TransportationMode);

}

public sealed record UiRejectShipmentReason(string RejectionReason)
{
    public static RejectShipmentReason ToDto(UiRejectShipmentReason source) =>
        new(source.RejectionReason);

}

public sealed record UiIssueShipmentReason(string IssueReason)
{
    public static IssueShipmentReason ToDto(UiIssueShipmentReason source) =>
        new(source.IssueReason);

}

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
    public UiPaymentSummaryDto? Payment { get; set; }
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
            Payment = source.Payment is null ? null : UiPaymentSummaryDto.ToUiDto(source.Payment),
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

public sealed class UiRetrieveShipmentSummaryDto
{
    public Guid Id { get; set; }
    public string TrackingNumber { get; set; } = string.Empty;
    public string ReceiverCity { get; set; } = string.Empty;
    public string SenderCity { get; set; } = string.Empty;
    public decimal ShippinCost { get; set; }
    public decimal Weight { get; set; }
    public ShipmentStatuses CurrentStatus { get; set; }
    public enShipmentType ShipmentType { get; set; }
    public DateTime? EstimatedDeliveryDate { get; set; }
    public DateTime CreatedAt { get; set; }

    public static UiRetrieveShipmentSummaryDto ToUiDto(RetrieveShipmentSummaryDto source) =>
        new()
        {
            Id = source.Id,
            TrackingNumber = source.TrackingNumber,
            ReceiverCity = source.ReceiverCity,
            SenderCity = source.SenderCity,
            ShippinCost = source.ShippinCost,
            Weight = source.Weight,
            CurrentStatus = source.CurrentStatus,
            ShipmentType = source.ShipmentType,
            EstimatedDeliveryDate = source.EstimatedDeliveryDate,
            CreatedAt = source.CreatedAt
        };

    public static UiPagedResult<UiRetrieveShipmentSummaryDto> ToUiPagedDto(
        PagedResult<RetrieveShipmentSummaryDto> source) =>
        UiPagedResult<UiRetrieveShipmentSummaryDto>.From(
            source.Data.Select(ToUiDto),
            source.TotalCount,
            source.PageNumber,
            source.PageSize);
}

public sealed class UiShipmentFilterDto
{
    public ShipmentStatuses[]? Status { get; set; }
    public TransportationMode? Mode { get; set; }
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
    public Guid? SenderId { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;

    public static ShipmentFilterDto ToDto(UiShipmentFilterDto source) =>
        new()
        {
            Status = source.Status is null ? null : [.. source.Status],
            Mode = source.Mode,
            From = source.From,
            To = source.To,
            SenderId = source.SenderId,
            PageNumber = source.PageNumber,
            PageSize = source.PageSize
        };

}
