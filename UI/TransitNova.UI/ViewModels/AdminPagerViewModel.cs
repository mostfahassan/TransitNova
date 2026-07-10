namespace TransitNova.UI.ViewModels;

public sealed class AdminPagerViewModel
{
    public int PageNumber { get; set; }
    public int TotalPages { get; set; }
    public string? PreviousUrl { get; set; }
    public string? NextUrl { get; set; }
}
