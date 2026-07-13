namespace TransitNova.UI.ViewModels;

public sealed record WarehouseSidebarItemViewModel(string Label, string Controller, string Action, string Icon, string? Badge = null);

public sealed class WarehousePageHeaderViewModel
{
    public string Eyebrow { get; set; } = "Warehouse";
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? PrimaryLabel { get; set; }
    public string? PrimaryController { get; set; }
    public string? PrimaryAction { get; set; }
    public IDictionary<string, string>? PrimaryRouteValues { get; set; }
    public string? SecondaryLabel { get; set; }
    public string? SecondaryController { get; set; }
    public string? SecondaryAction { get; set; }
    public IDictionary<string, string>? SecondaryRouteValues { get; set; }
}

public sealed record WarehouseKpiTileViewModel(string Label, string Value, string Meta, string Tone = "neutral", decimal? Progress = null);

public sealed record WarehouseDisplayCellViewModel(string Header, string Text, string Kind = "text");

public sealed class WarehouseTableRowViewModel
{
    public IReadOnlyCollection<WarehouseDisplayCellViewModel> Cells { get; init; } = [];
    public IDictionary<string, object?> RouteValues { get; init; } = new Dictionary<string, object?>();
}

public sealed class WarehouseTableViewModel
{
    public string Title { get; set; } = string.Empty;
    public string Subtitle { get; set; } = string.Empty;
    public IReadOnlyCollection<string> Headers { get; set; } = [];
    public IReadOnlyList<WarehouseTableRowViewModel> Rows { get; set; } = [];
    public int TotalCount { get; set; }
    public int PageNumber { get; set; } = 1;
    public int TotalPages { get; set; } = 1;
    public string Controller { get; set; } = string.Empty;
    public string DetailsAction { get; set; } = "Details";
    public bool ShowDetails { get; set; } = true;
    public string EmptyTitle { get; set; } = "No warehouse records";
    public string EmptyDescription { get; set; } = "Records from the backend will appear here when available.";
}

public sealed class WarehouseDetailViewModel
{
    public string Title { get; set; } = string.Empty;
    public string Subtitle { get; set; } = string.Empty;
    public string PrimaryLabel { get; set; } = string.Empty;
    public IReadOnlyCollection<WarehouseDisplayCellViewModel> Fields { get; set; } = [];
    public IReadOnlyCollection<WarehouseDisplayCellViewModel> AdditionalFields { get; set; } = [];
    public string Controller { get; set; } = string.Empty;
    public string IndexAction { get; set; } = "Index";
}
