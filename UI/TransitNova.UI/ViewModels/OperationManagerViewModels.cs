namespace TransitNova.UI.ViewModels;

public sealed class OpsPageHeaderViewModel
{
    public string Eyebrow { get; set; } = "Operations";
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? PrimaryLabel { get; set; }
    public string? PrimaryController { get; set; }
    public string? PrimaryAction { get; set; }
    public object? PrimaryRouteValues { get; set; }
    public string? SecondaryLabel { get; set; }
    public string? SecondaryController { get; set; }
    public string? SecondaryAction { get; set; }
    public object? SecondaryRouteValues { get; set; }
}

public sealed record OpsSidebarItemViewModel(string Label, string Controller, string Action, string Icon, string? Badge = null);

public sealed record OpsKpiTileViewModel(string Label, string Value, string Meta, string Icon, string Tone = "neutral");

public sealed record OpsTableColumnViewModel(string Header, string PropertyPath, string Kind = "text");

public sealed class OpsTableViewModel
{
    public string Title { get; set; } = string.Empty;
    public string Subtitle { get; set; } = string.Empty;
    public object? Source { get; set; }
    public IReadOnlyCollection<OpsTableColumnViewModel> Columns { get; set; } = [];
    public string Controller { get; set; } = string.Empty;
    public string DetailsAction { get; set; } = "Details";
    public string RouteIdName { get; set; } = "id";
    public string IdPropertyPath { get; set; } = "Id";
    public bool ShowDetails { get; set; } = true;
    public string? PrimaryRowLabel { get; set; }
    public string? PrimaryRowAction { get; set; }
    public string? PrimaryRowController { get; set; }
    public string PrimaryRowRouteIdName { get; set; } = "id";
    public string PrimaryRowIdPropertyPath { get; set; } = "Id";
    public string EmptyTitle { get; set; } = "No operational records";
    public string EmptyDescription { get; set; } = "Live records from the API will appear here when available.";
}

public sealed class OpsDetailViewModel
{
    public string Title { get; set; } = string.Empty;
    public string Subtitle { get; set; } = string.Empty;
    public object? Source { get; set; }
    public IReadOnlyCollection<OpsTableColumnViewModel> Fields { get; set; } = [];
    public string Controller { get; set; } = string.Empty;
    public string IndexAction { get; set; } = "Index";
    public string? PrimaryLabel { get; set; }
    public string? PrimaryAction { get; set; }
    public string? SecondaryLabel { get; set; }
    public string? SecondaryAction { get; set; }
    public string RouteIdName { get; set; } = "id";
    public string IdPropertyPath { get; set; } = "Id";
}

public sealed class OpsAssignCarrierPageViewModel
{
    public Guid ShipmentId { get; set; }

    [System.ComponentModel.DataAnnotations.Required]
    public Guid CarrierId { get; set; }

    public string AssignmentType { get; set; } = "Pickup";
    public string? City { get; set; }
    public int? CityId { get; set; }
    public DateTime? ScheduledAt { get; set; } = DateTime.UtcNow.AddHours(1);
    public object? Carriers { get; set; }

    public TransitNovaUI.BusinessLayer.DTOs.Carrier.UiAssignCarrierDto ToDto() => new(CarrierId);
}