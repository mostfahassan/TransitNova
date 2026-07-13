using TransitNovaUI.BusinessLayer.DTOs.Bundle;
using TransitNovaUI.BusinessLayer.DTOs.OperationManager;
using TransitNovaUI.BusinessLayer.DTOs.Payment;

namespace TransitNova.UI.ViewModels;

public sealed record UserDashboardPageViewModel(
    UiProfileDashboardDto? Dashboard,
    IReadOnlyCollection<UiInvoiceDto> RecentInvoices,
    int TotalInvoices,
    IReadOnlyCollection<UiRetrieveBundleDto> AvailableBundles);