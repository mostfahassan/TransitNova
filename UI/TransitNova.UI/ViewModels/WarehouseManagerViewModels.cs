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

public sealed record WarehouseTableColumnViewModel(string Header, string PropertyPath, string Kind = "text");

public sealed class WarehouseTableViewModel
{
    public string Title { get; set; } = string.Empty;
    public string Subtitle { get; set; } = string.Empty;
    public object? Source { get; set; }
    public IReadOnlyCollection<WarehouseTableColumnViewModel> Columns { get; set; } = [];
    public string Controller { get; set; } = string.Empty;
    public string DetailsAction { get; set; } = "Details";
    public string RouteIdName { get; set; } = "id";
    public string IdPropertyPath { get; set; } = "Id";
    public bool ShowDetails { get; set; } = true;
    public string EmptyTitle { get; set; } = "No warehouse records";
    public string EmptyDescription { get; set; } = "Records from the backend will appear here when available.";
}

public sealed class WarehouseDetailViewModel
{
    public string Title { get; set; } = string.Empty;
    public string Subtitle { get; set; } = string.Empty;
    public object? Source { get; set; }
    public IReadOnlyCollection<WarehouseTableColumnViewModel> Fields { get; set; } = [];
    public string Controller { get; set; } = string.Empty;
    public string IndexAction { get; set; } = "Index";
    public string RouteIdName { get; set; } = "id";
    public string IdPropertyPath { get; set; } = "Id";
}