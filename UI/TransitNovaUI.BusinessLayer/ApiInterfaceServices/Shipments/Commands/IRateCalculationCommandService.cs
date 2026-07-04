namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Shipments.Commands;

public interface IRateCalculationCommandService
{
    Task<ApiResponse<decimal>> CalculateRateAsync(UiRateCalculatorDto model, CancellationToken cancellationToken = default);
}
