using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using TransitNova.UI.Infrastructure.Mvc.Common;
using TransitNova.UI.Infrastructure.Mvc.Interface;
using TransitNova.UI.ViewModels;
using TransitNovaUI.BusinessLayer.ApiContracts;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Authentication.Segregation;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Shared.Locations.Queries;
using TransitNovaUI.BusinessLayer.DTOs.City;
using TransitNovaUI.BusinessLayer.DTOs.Country;
using TransitNovaUI.BusinessLayer.DTOs.UserProfile.Auth;

namespace TransitNova.UI.Areas.AccountArea.Controllers;

[Area("AccountArea")]
[Route("[area]/[controller]/[action]")]
public sealed class AccountController(
    IBackendApiInvoker apiInvoker,
    IUiAuthSessionService authSession,
    IRoleHomeResolver roleHomeResolver,
    IAuthenticationCommand authenticationCommand,
    IGetCountriesQueryService countriesQuery,
    IGetCountryGovernmentsQueryService countryGovernmentsQuery,
    IGetCitiesByGovernmentQueryService citiesByGovernmentQuery)
    : AppControllerBase
{
    [AllowAnonymous]
    [HttpGet]
    public IActionResult Login() => View(new LoginViewModel());

    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return IsAjaxRequest() ? AjaxValidationFailure() : View(model);

        var response = await apiInvoker.ExecuteAsync((_, ct) => authenticationCommand.LoginAsync(model.ToDto(), ct), requiresAuthentication: false, cancellationToken);

        if (response.IsFailure || response.Data is null)
        {
            AddApiErrors(response);
            return IsAjaxRequest() ? AjaxApiFailure(response) : View(model);
        }

        await authSession.SignInAsync(response.Data, cancellationToken);
        var redirectUrl = roleHomeResolver.Resolve(response.Data.Roles.FirstOrDefault());

        return IsAjaxRequest() ? AjaxRedirect(redirectUrl) : RedirectToLocalPath(redirectUrl);
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> Register(CancellationToken cancellationToken)
    {
        var model = new RegisterViewModel();
        await PopulateRegisterLocationsAsync(model, cancellationToken);
        return View(model);
    }

    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            if (IsAjaxRequest())
                return AjaxValidationFailure();

            await PopulateRegisterLocationsAsync(model, cancellationToken);
            return View(model);
        }

        ApiResponse<UiAuthResponseDto> response;
        try
        {
            response = await apiInvoker.ExecuteAsync((_, ct) => authenticationCommand.RegisterAsync(model.ToDto(), ct), requiresAuthentication: false, cancellationToken);
        }
        catch (HttpRequestException ex)
        {
            return BackendUnavailable(ex);
        }

        if (response.IsFailure || response.Data is null)
        {
            AddApiErrors(response);
            if (IsAjaxRequest())
                return AjaxApiFailure(response);

            await PopulateRegisterLocationsAsync(model, cancellationToken);
            return View(model);
        }

        await authSession.SignInAsync(response.Data, cancellationToken);
        var redirectUrl = roleHomeResolver.Resolve(response.Data.Roles.FirstOrDefault());

        return IsAjaxRequest() ? AjaxRedirect(redirectUrl) : RedirectToLocalPath(redirectUrl);
    }

    [Authorize]
    [HttpGet]
    public IActionResult ChangePassword() => View(new ChangePasswordViewModel());

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return IsAjaxRequest() ? AjaxValidationFailure() : View(model);

        var response = await apiInvoker.ExecuteAsync((token, ct) => authenticationCommand.ChangePasswordAsync(model.ToDto(), token!, ct), cancellationToken: cancellationToken);

        if (response.IsFailure)
        {
            AddApiErrors(response);
            return IsAjaxRequest() ? AjaxApiFailure(response) : View(model);
        }

        Success("Password changed successfully.");
        var redirectUrl = Url.Action(nameof(ChangePassword), "Account", new { area = "AccountArea" }) ?? "/AccountArea/Account/ChangePassword";

        return IsAjaxRequest() ? AjaxRedirect(redirectUrl, "Password changed successfully.") : RedirectToAction(nameof(ChangePassword));
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        await apiInvoker.ExecuteAsync((token, ct) => authenticationCommand.SignOutAsync(token!, ct), cancellationToken: cancellationToken);

        await authSession.SignOutAsync();
        return RedirectToAction(nameof(Login));
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult AccessDenied() => View();

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> Countries(CancellationToken cancellationToken)
    {
        try
        {
            var response = await apiInvoker.ExecuteAsync((_, ct) => countriesQuery.GetCountriesAsync(string.Empty, ct), requiresAuthentication: false, cancellationToken);

            if (response.IsFailure)
                return LocationFailure(response);

            return Json(new { items = ToLocationOptions(response.Data) });
        }
        catch (HttpRequestException)
        {
            return LocationUnavailable();
        }
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> Governments(int countryId, CancellationToken cancellationToken)
    {
        if (countryId <= 0)
            return BadRequest(new { message = "Country is required.", items = Array.Empty<LocationOptionViewModel>() });

        try
        {
            var response = await apiInvoker.ExecuteAsync((_, ct) => countryGovernmentsQuery.GetCountryGovernmentsAsync(countryId, string.Empty, ct), requiresAuthentication: false, cancellationToken);

            if (response.IsFailure)
                return LocationFailure(response);

            return Json(new { items = ToLocationOptions(response.Data) });
        }
        catch (HttpRequestException)
        {
            return LocationUnavailable();
        }
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> Cities(int governmentId, CancellationToken cancellationToken)
    {
        if (governmentId <= 0)
            return BadRequest(new { message = "Government is required.", items = Array.Empty<LocationOptionViewModel>() });

        try
        {
            var response = await apiInvoker.ExecuteAsync((_, ct) => citiesByGovernmentQuery.GetCitiesByGovernmentAsync(governmentId, string.Empty, ct), requiresAuthentication: false, cancellationToken);

            if (response.IsFailure)
                return LocationFailure(response);

            return Json(new { items = ToLocationOptions(response.Data) });
        }
        catch (HttpRequestException)
        {
            return LocationUnavailable();
        }
    }

    private async Task PopulateRegisterLocationsAsync(RegisterViewModel model, CancellationToken cancellationToken)
    {
        try
        {
            var countriesResponse = await apiInvoker.ExecuteAsync((_, ct) => countriesQuery.GetCountriesAsync(string.Empty, ct), requiresAuthentication: false, cancellationToken);
            model.Countries = ToLocationOptions(countriesResponse.Data);

            if (model.CountryId > 0)
            {
                var governmentsResponse = await apiInvoker.ExecuteAsync((_, ct) => countryGovernmentsQuery.GetCountryGovernmentsAsync(model.CountryId, string.Empty, ct), requiresAuthentication: false, cancellationToken);
                model.Governments = ToLocationOptions(governmentsResponse.Data);
            }

            if (model.GovernmentId > 0)
            {
                var citiesResponse = await apiInvoker.ExecuteAsync((_, ct) => citiesByGovernmentQuery.GetCitiesByGovernmentAsync(model.GovernmentId, string.Empty, ct), requiresAuthentication: false, cancellationToken);
                model.Cities = ToLocationOptions(citiesResponse.Data);
            }
        }
        catch (HttpRequestException)
        {
            Error("Location data is temporarily unavailable. Try again after the API is running.");
        }
    }

    private bool IsAjaxRequest() =>
        string.Equals(Request.Headers.XRequestedWith, "XMLHttpRequest", StringComparison.OrdinalIgnoreCase)
        || Request.Headers.Accept.Any(value => value?.Contains("application/json", StringComparison.OrdinalIgnoreCase) == true);

    private IActionResult AjaxValidationFailure() =>
        UnprocessableEntity(new { isSuccess = false, message = "Please review the highlighted fields.", errors = ModelStateErrors() });

    private IActionResult AjaxApiFailure(ApiResponse response)
    {
        var errors = response.Errors.Count > 0
            ? response.Errors.Select(error => error.Message).ToArray()
            : [ApiMessage(response)];

        return StatusCode(response.StatusCode, new { isSuccess = false, message = ApiMessage(response), errors = new Dictionary<string, string[]> { [string.Empty] = errors } });
    }

    private JsonResult AjaxRedirect(string redirectUrl, string? message = null) =>
        Json(new { isSuccess = true, redirectUrl, message });

    private IActionResult LocationFailure(ApiResponse response) =>
        StatusCode(response.StatusCode, new { message = ApiMessage(response), items = Array.Empty<LocationOptionViewModel>() });

    private IActionResult LocationUnavailable() =>
        StatusCode(StatusCodes.Status503ServiceUnavailable, new { message = "Location service is unavailable.", items = Array.Empty<LocationOptionViewModel>() });

    private IActionResult BackendUnavailable(HttpRequestException exception)
    {
        var message = $"Backend API is unavailable. {exception.Message}";
        if (IsAjaxRequest())
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new { isSuccess = false, message, errors = new Dictionary<string, string[]> { [string.Empty] = [message] } });

        Error(message);
        return View("Register", new RegisterViewModel());
    }

    private Dictionary<string, string[]> ModelStateErrors() =>
        ModelState
            .Where(item => item.Value?.ValidationState == ModelValidationState.Invalid)
            .ToDictionary(
                item => item.Key,
                item => item.Value?.Errors.Select(error => string.IsNullOrWhiteSpace(error.ErrorMessage) ? "Invalid value." : error.ErrorMessage).ToArray() ?? []);

    private static string ApiMessage(ApiResponse response) =>
        response.Error?.Message
        ?? response.Errors.FirstOrDefault()?.Message
        ?? response.Message
        ?? "The request could not be completed.";

    private static IReadOnlyCollection<LocationOptionViewModel> ToLocationOptions(IEnumerable<UiCountryDto>? countries) =>
        countries?.Select(country => new LocationOptionViewModel(country.Id, country.Name)).ToArray() ?? [];

    private static IReadOnlyCollection<LocationOptionViewModel> ToLocationOptions(IEnumerable<UiGovernmentDto>? governments) =>
        governments?.Select(government => new LocationOptionViewModel(government.Id, government.Name)).ToArray() ?? [];

    private static IReadOnlyCollection<LocationOptionViewModel> ToLocationOptions(IEnumerable<UiCityDto>? cities) =>
        cities?.Select(city => new LocationOptionViewModel(city.Id, city.Name)).ToArray() ?? [];
}


