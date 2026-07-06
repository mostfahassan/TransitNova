using Microsoft.AspNetCore.Mvc.ModelBinding;
using TransitNovaUI.BusinessLayer.ApiContracts;

namespace TransitNova.UI.Infrastructure.Mvc.Implementation;

public static class ApiResponseModelStateExtensions
{
    public static void AddApiErrors(this ModelStateDictionary modelState, ApiResponse response)
    {
        if (response.Error is not null)
            modelState.AddModelError(string.Empty, response.Error.Message);

        foreach (var error in response.Errors)
            modelState.AddModelError(string.Empty, error.Message);

        if (response.Error is null && response.Errors.Count == 0 && !string.IsNullOrWhiteSpace(response.Message))
            modelState.AddModelError(string.Empty, response.Message);
    }
}
