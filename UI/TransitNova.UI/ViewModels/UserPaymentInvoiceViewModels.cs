using TransitNovaUI.BusinessLayer.DTOs.Payment;

namespace TransitNova.UI.ViewModels;

public sealed record UserDashboardPageViewModel(
    object? Dashboard,
    IReadOnlyCollection<UiInvoiceDto> RecentInvoices,
    int TotalInvoices);

public sealed record UserPaymentInvoicesIndexViewModel(
    IReadOnlyCollection<UiInvoiceDto> Invoices);

public sealed record UserPaymentInvoiceDetailsViewModel(
    UiInvoiceDto Invoice);
