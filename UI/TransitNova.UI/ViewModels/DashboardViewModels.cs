namespace TransitNova.UI.ViewModels;

public sealed record DashboardNavItemViewModel(
    string Label,
    string Href,
    string Icon = "grid",
    bool IsActive = false,
    string? Badge = null);

public sealed record DashboardKpiCardViewModel(
    string Title,
    decimal Value,
    string DisplayValue,
    string Icon = "activity",
    string TrendLabel = "+0%",
    bool IsPositiveTrend = true,
    string? SupportingText = null);

public sealed record DashboardChartWidgetViewModel(
    string Title,
    string Subtitle,
    string CanvasId,
    string FilterLabel = "Last 30 days");

public sealed record DashboardTableViewModel(
    string Title,
    string Subtitle,
    IReadOnlyCollection<DashboardTableRowViewModel> Rows);

public sealed record DashboardTableRowViewModel(
    string ShipmentId,
    string Origin,
    string Destination,
    string Status,
    string UpdatedAt,
    string ActionLabel,
    string ActionUrl = "#");
