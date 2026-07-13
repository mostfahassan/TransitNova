namespace TransitNova.UI.ViewModels;

public sealed class AdminPageHeaderViewModel
{
    public string Eyebrow { get; set; } = "Admin Console";
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? PrimaryLabel { get; set; }
    public string? PrimaryController { get; set; }
    public string? PrimaryAction { get; set; }
    public string? SecondaryLabel { get; set; }
    public string? SecondaryController { get; set; }
    public string? SecondaryAction { get; set; }
}

public sealed record AdminSidebarItemViewModel(string Label, string Controller, string Action, string Icon, string? Badge = null);

public sealed record AdminKpiTileViewModel(string Label, string Value, string Meta, string Icon, string Tone = "neutral");

public sealed record AdminDisplayCellViewModel(string Header, string Text, string Kind = "text");

public sealed class AdminTableRowViewModel
{
    public IReadOnlyCollection<AdminDisplayCellViewModel> Cells { get; init; } = [];
    public IDictionary<string, object?> RouteValues { get; init; } = new Dictionary<string, object?>();
}

public sealed record AdminRowActionViewModel(string Label, string Action, string Style = "secondary");

public sealed class AdminTableViewModel
{
    public string Title { get; set; } = string.Empty;
    public string Subtitle { get; set; } = string.Empty;
    public IReadOnlyCollection<string> Headers { get; set; } = [];
    public IReadOnlyList<AdminTableRowViewModel> Rows { get; set; } = [];
    public int TotalCount { get; set; }
    public int PageNumber { get; set; } = 1;
    public int TotalPages { get; set; } = 1;
    public IReadOnlyCollection<AdminRowActionViewModel> RowActions { get; set; } = [];
    public string Controller { get; set; } = string.Empty;
    public string DetailsAction { get; set; } = "Details";
    public string EditAction { get; set; } = "Edit";
    public string DeleteAction { get; set; } = "Delete";
    public bool ShowDetails { get; set; } = true;
    public bool ShowEdit { get; set; }
    public bool ShowDelete { get; set; }
    public string EmptyTitle { get; set; } = "No records found";
    public string EmptyDescription { get; set; } = "Once data is available from the API it will appear here.";
}

public sealed class AdminDetailViewModel
{
    public string Title { get; set; } = string.Empty;
    public string Subtitle { get; set; } = string.Empty;
    public string PrimaryLabel { get; set; } = string.Empty;
    public IReadOnlyCollection<AdminDisplayCellViewModel> Fields { get; set; } = [];
    public IReadOnlyCollection<AdminDisplayCellViewModel> AdditionalFields { get; set; } = [];
    public IDictionary<string, object?> RouteValues { get; set; } = new Dictionary<string, object?>();
    public string Controller { get; set; } = string.Empty;
    public string IndexAction { get; set; } = "Index";
    public string? EditAction { get; set; }
    public string? DeleteAction { get; set; }
}
