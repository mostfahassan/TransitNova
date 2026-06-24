using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Carrier;
using TransitNova.Domain.Enums.Carrier;
using TransitNova.Domain.Enums.Users;
using TransitNovaUI.BusinessLayer.Common.ResultPattern;
namespace TransitNovaUI.BusinessLayer.DTOs.Carrier;

public sealed class UiCarrierSummaryDetailsDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string? Code { get; set; }
    public CarrierStatus Status { get; set; }
    public List<string> ServedCities { get; set; } = [];
    public int AssignedShipmentsCount { get; set; }
    public int ActiveTripsCount { get; set; }
    public decimal Rating { get; set; }

    public static UiCarrierSummaryDetailsDto ToUiDto(CarrierSummaryDetailsDto source) =>
        new()
        {
            Id = source.Id,
            FullName = source.FullName,
            PhoneNumber = source.PhoneNumber,
            Code = source.Code,
            Status = source.Status,
            ServedCities = [.. source.ServedCities],
            AssignedShipmentsCount = source.AssignedShipmentsCount,
            ActiveTripsCount = source.ActiveTripsCount,
            Rating = source.Rating
        };

    public static UiPagedResult<UiCarrierSummaryDetailsDto> ToUiPagedDto(
        PagedResult<CarrierSummaryDetailsDto> source) =>
        UiPagedResult<UiCarrierSummaryDetailsDto>.From(
            source.Data.Select(ToUiDto),
            source.TotalCount,
            source.PageNumber,
            source.PageSize);
}
