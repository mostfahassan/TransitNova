using TransitNova.BusinessLayer.DTOs.WarehouseManager;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNovaUI.BusinessLayer.Common.ResultPattern;

namespace TransitNovaUI.BusinessLayer.DTOs.WarehouseManager;

public sealed class UiWarehouseManagerListDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public Guid? WarehouseId { get; set; }
    public string? WarehouseName { get; set; }

    public static UiWarehouseManagerListDto ToUiDto(WarehouseManagerListDto source) =>
        new()
        {
            Id = source.Id,
            FullName = source.FullName,
            Email = source.Email,
            WarehouseId = source.WarehouseId,
            WarehouseName = source.WarehouseName
        };

    public static UiPagedResult<UiWarehouseManagerListDto> ToUiPagedDto(PagedResult<WarehouseManagerListDto> source) =>
        UiPagedResult<UiWarehouseManagerListDto>.From(source.Data.Select(ToUiDto), source.TotalCount, source.PageNumber, source.PageSize);
}
