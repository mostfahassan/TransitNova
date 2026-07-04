using Microsoft.AspNetCore.Mvc.ViewFeatures;
using TransitNovaUI.BusinessLayer.ApiContracts;

namespace TransitNova.UI.Infrastructure.Mvc;

public static class TempDataExtensions
{
    private const string SuccessKey = "SuccessMessage";
    private const string ErrorKey = "ErrorMessage";

    public static void SetSuccess(this ITempDataDictionary tempData, string message) =>
        tempData[SuccessKey] = message;

    public static void SetError(this ITempDataDictionary tempData, string message) =>
        tempData[ErrorKey] = message;

    public static void SetApiError(this ITempDataDictionary tempData, ApiResponse response)
    {
        var message = response.Error?.Message
            ?? response.Errors.FirstOrDefault()?.Message
            ?? response.Message
            ?? "The operation could not be completed.";

        tempData.SetError(message);
    }
}
