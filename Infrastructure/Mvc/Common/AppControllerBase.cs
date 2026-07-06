using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using TransitNova.Domain.Enums.Result;
using TransitNova.UI.Infrastructure.Mvc.Implementation;
using TransitNovaUI.BusinessLayer.ApiContracts;

namespace TransitNova.UI.Infrastructure.Mvc.Common;

public abstract class AppControllerBase : Controller
{
    protected Guid? CurrentUserId =>
        Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId)
            ? userId
            : null;

    protected string? CurrentRole =>
        User.FindFirstValue(ClaimTypes.Role);

    protected Guid? CurrentWarehouseId
    {
        get
        {
            var value = HttpContext.Session.GetString(SessionKeys.WarehouseId);
            return Guid.TryParse(value, out var warehouseId) ? warehouseId : null;
        }
    }

    protected void SetCurrentWarehouseId(Guid warehouseId) =>
        HttpContext.Session.SetString(SessionKeys.WarehouseId, warehouseId.ToString());
    protected Guid? CurrentCarrierId
    {
        get
        {
            var value = HttpContext.Session.GetString(SessionKeys.CarrierId);
            return Guid.TryParse(value, out var carrierId) ? carrierId : null;
        }
    }

    protected Guid? ResolvedCarrierId =>
        CurrentCarrierId ?? CurrentUserId;

    protected void SetCurrentCarrierId(Guid carrierId) =>
        HttpContext.Session.SetString(SessionKeys.CarrierId, carrierId.ToString());

    protected void AddApiErrors(ApiResponse response) =>
        ModelState.AddApiErrors(response);

    protected void Success(string message) =>
        TempData.SetSuccess(message);

    protected void Error(string message) =>
        TempData.SetError(message);

    protected void ApiError(ApiResponse response) =>
        TempData.SetApiError(response);

    protected IActionResult HandleGetFailure(ApiResponse response)
    {
        return response.Status switch
        {
            ResultStatus.Unauthorized => Challenge(),
            ResultStatus.Forbidden => Forbid(),
            ResultStatus.NotFound => RedirectToAction("NotFound", "Errors", new { area = "AccountArea" }),
            ResultStatus.UnExpected => RedirectToAction("ServerError", "Errors", new { area = "AccountArea" }),
            _ => Problem(response.Error?.Message ?? response.Message ?? "The request failed.", statusCode: response.StatusCode)
        };
    }

    protected IActionResult RedirectToLocalPath(string path) =>
        LocalRedirect(path);
}

