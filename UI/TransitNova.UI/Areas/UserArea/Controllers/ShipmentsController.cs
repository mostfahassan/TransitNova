using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using TransitNova.Domain.Contracts.Roles;
using TransitNova.Domain.Enums.Result;
using TransitNova.UI.ViewModels;
using TransitNovaUI.BusinessLayer.ApiContracts;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.User.Shipments.Segregation;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Shared.Locations.Queries;
using TransitNovaUI.BusinessLayer.DTOs.Payment;
using TransitNovaUI.BusinessLayer.DTOs.City;
using TransitNovaUI.BusinessLayer.DTOs.Country;

namespace TransitNova.UI.Areas.UserArea.Controllers;

[Authorize(Roles = Role.User)]
[Area("UserArea")]
[Route("[area]/[controller]/[action]")]
public sealed class ShipmentsController(
    IBackendApiInvoker apiInvoker,
    IIdempotencyKeyFactory idempotencyKeyFactory,
    IUserShipmentsQuery userShipmentsQuery,
    IUserShipmentsCommand userShipmentsCommand,
    IGetCountriesQueryService countriesQuery,
    IGetCountryGovernmentsQueryService countryGovernmentsQuery,
    IGetCitiesByGovernmentQueryService citiesByGovernmentQuery)
    : AppControllerBase
{
    private const string PaymentFailureFallbackMessage = "No invoice was generated. Please review the payment method and try again.";

    [HttpGet]
    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        var model = new CreateShipmentViewModel();
        await PopulateShipmentLocationsAsync(model, cancellationToken);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateShipmentViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            if (IsAjaxRequest())
                return AjaxValidationFailure();

            await PopulateShipmentLocationsAsync(model, cancellationToken);
            return View(model);
        }

        var senderId = CurrentUserId;
        if (senderId is null)
            return Challenge();

        var response = await apiInvoker.ExecuteAsync((token, ct) => userShipmentsCommand.CreateShipmentAsync(model.ToDto(senderId.Value), token!, idempotencyKeyFactory.Create(), ct), cancellationToken: cancellationToken);

        if (response.IsFailure || response.Data is null)
        {
            AddApiErrors(response);
            if (IsAjaxRequest())
                return AjaxApiFailure(response);

            await PopulateShipmentLocationsAsync(model, cancellationToken);
            return View(model);
        }

        var shipmentDetailsUrl = Url.Action(nameof(Details), new { shipmentId = response.Data.ShipmentId })
            ?? $"/UserArea/Shipments/Details/{response.Data.ShipmentId}";

        if (IsAjaxRequest())
        {
            return StatusCode(
                response.StatusCode is >= 200 and < 300 ? response.StatusCode : StatusCodes.Status200OK,
                new CreateShipmentFlowResponse(
                    true,
                    "Invoice ready",
                    "Your payment was processed and the invoice is ready.",
                    response.Data,
                    shipmentDetailsUrl,
                    null));
        }

        Success("Shipment created successfully.");
        return RedirectToAction(nameof(Details), new { shipmentId = response.Data.ShipmentId });
    }

    [HttpGet]
    public async Task<IActionResult> Governments(int countryId, CancellationToken cancellationToken)
    {
        if (countryId <= 0)
            return BadRequest(new { message = "Country is required.", items = Array.Empty<LocationOptionViewModel>() });

        try
        {
            var response = await apiInvoker.ExecuteAsync((token, ct) => countryGovernmentsQuery.GetCountryGovernmentsAsync(countryId, token!, ct), cancellationToken: cancellationToken);
            if (response.IsFailure)
                return LocationFailure(response);

            return Json(new { items = ToLocationOptions(response.Data) });
        }
        catch (HttpRequestException)
        {
            return LocationUnavailable();
        }
    }

    [HttpGet]
    public async Task<IActionResult> Cities(int governmentId, CancellationToken cancellationToken)
    {
        if (governmentId <= 0)
            return BadRequest(new { message = "Government is required.", items = Array.Empty<LocationOptionViewModel>() });

        try
        {
            var response = await apiInvoker.ExecuteAsync((token, ct) => citiesByGovernmentQuery.GetCitiesByGovernmentAsync(governmentId, token!, ct), cancellationToken: cancellationToken);
            if (response.IsFailure)
                return LocationFailure(response);

            return Json(new { items = ToLocationOptions(response.Data) });
        }
        catch (HttpRequestException)
        {
            return LocationUnavailable();
        }
    }

    [HttpGet("{shipmentId:guid}")]
    public async Task<IActionResult> Details(Guid shipmentId, CancellationToken cancellationToken)
    {
        var response = await apiInvoker.ExecuteAsync((token, ct) => userShipmentsQuery.GetUserShipmentByIdAsync(shipmentId, token!, ct), cancellationToken: cancellationToken);

        return response.IsSuccess ? View(response.Data) : HandleGetFailure(response);
    }

    [HttpGet("{shipmentId:guid}")]
    public async Task<IActionResult> Edit(Guid shipmentId, CancellationToken cancellationToken)
    {
        var response = await apiInvoker.ExecuteAsync((token, ct) => userShipmentsQuery.GetUserShipmentByIdAsync(shipmentId, token!, ct), cancellationToken: cancellationToken);

        if (response.IsFailure)
            return HandleGetFailure(response);

        return response.Data is null
            ? RedirectToAction("NotFound", "Errors", new { area = "AccountArea" })
            : View(PrefillViewModelFactory.Shipment(response.Data));
    }

    [HttpPost("{shipmentId:guid}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid shipmentId, UpdateShipmentViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return View(model);

        var response = await apiInvoker.ExecuteAsync((token, ct) => userShipmentsCommand.UpdateShipmentAsync(shipmentId, model.ToDto(), token!, idempotencyKeyFactory.Create(), ct), cancellationToken: cancellationToken);

        if (response.IsFailure)
        {
            AddApiErrors(response);
            return View(model);
        }

        Success("Shipment updated successfully.");
        return RedirectToAction(nameof(Details), new { shipmentId });
    }

    [HttpGet]
    public IActionResult Track() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult TrackSubmit(string trackingNumber)
    {
        if (string.IsNullOrWhiteSpace(trackingNumber))
        {
            ModelState.AddModelError(nameof(trackingNumber), "Tracking number is required.");
            return View("Track");
        }

        return RedirectToAction(nameof(TrackResult), new { trackingNumber });
    }

    [HttpGet("{trackingNumber}")]
    public async Task<IActionResult> TrackResult(string trackingNumber, CancellationToken cancellationToken)
    {
        var response = await apiInvoker.ExecuteAsync((token, ct) => userShipmentsQuery.TrackShipmentAsync(trackingNumber, token!, ct), cancellationToken: cancellationToken);

        return response.IsSuccess ? View(response.Data) : HandleGetFailure(response);
    }

    [HttpGet("{shipmentId:guid}")]
    public IActionResult Issue(Guid shipmentId) => View(new ReasonViewModel());

    [HttpPost("{shipmentId:guid}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Issue(Guid shipmentId, ReasonViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return View(model);

        var response = await apiInvoker.ExecuteAsync((token, ct) => userShipmentsCommand.IssueShipmentAsync(shipmentId, model.ToIssueDto(), token!, idempotencyKeyFactory.Create(), ct), cancellationToken: cancellationToken);

        if (response.IsFailure)
        {
            AddApiErrors(response);
            return View(model);
        }

        Success("Shipment issue submitted successfully.");
        return RedirectToAction(nameof(Details), new { shipmentId });
    }

    [HttpPost("{shipmentId:guid}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(Guid shipmentId, CancellationToken cancellationToken)
    {
        var response = await apiInvoker.ExecuteAsync((token, ct) => userShipmentsCommand.CancelShipmentAsync(shipmentId, token!, idempotencyKeyFactory.Create(), ct), cancellationToken: cancellationToken);

        if (response.IsFailure)
            ApiError(response);
        else
            Success("Shipment cancelled successfully.");

        return RedirectToAction(nameof(Details), new { shipmentId });
    }

    [HttpPost("{shipmentId:guid}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid shipmentId, CancellationToken cancellationToken)
    {
        var response = await apiInvoker.ExecuteAsync((token, ct) => userShipmentsCommand.DeleteShipmentAsync(shipmentId, token!, idempotencyKeyFactory.Create(), ct), cancellationToken: cancellationToken);

        if (response.IsFailure)
        {
            ApiError(response);
            return RedirectToAction(nameof(Details), new { shipmentId });
        }

        Success("Shipment deleted successfully.");
        return RedirectToAction(nameof(DashboardController.Index), "Dashboard");
    }

    private async Task PopulateShipmentLocationsAsync(CreateShipmentViewModel model, CancellationToken cancellationToken)
    {
        try
        {
            var countriesResponse = await apiInvoker.ExecuteAsync((token, ct) => countriesQuery.GetCountriesAsync(token!, ct), cancellationToken: cancellationToken);
            model.Countries = ToLocationOptions(countriesResponse.Data);

            if (model.CountryId > 0)
            {
                var governmentsResponse = await apiInvoker.ExecuteAsync((token, ct) => countryGovernmentsQuery.GetCountryGovernmentsAsync(model.CountryId, token!, ct), cancellationToken: cancellationToken);
                model.Governments = ToLocationOptions(governmentsResponse.Data);
            }

            if (model.GovernmentId > 0)
            {
                var citiesResponse = await apiInvoker.ExecuteAsync((token, ct) => citiesByGovernmentQuery.GetCitiesByGovernmentAsync(model.GovernmentId, token!, ct), cancellationToken: cancellationToken);
                model.Cities = ToLocationOptions(citiesResponse.Data);
            }
        }
        catch (HttpRequestException)
        {
            Error("Location data is temporarily unavailable. Try again after the API is running.");
        }
    }

    private IActionResult LocationFailure(ApiResponse response) =>
        StatusCode(response.StatusCode, new { message = ResolveApiMessage(response), items = Array.Empty<LocationOptionViewModel>() });

    private IActionResult LocationUnavailable() =>
        StatusCode(StatusCodes.Status503ServiceUnavailable, new { message = "Location service is unavailable.", items = Array.Empty<LocationOptionViewModel>() });

    private static IReadOnlyCollection<LocationOptionViewModel> ToLocationOptions(IEnumerable<UiCountryDto>? countries) =>
        countries?.Select(country => new LocationOptionViewModel(country.Id, country.Name)).ToArray() ?? [];

    private static IReadOnlyCollection<LocationOptionViewModel> ToLocationOptions(IEnumerable<UiGovernmentDto>? governments) =>
        governments?.Select(government => new LocationOptionViewModel(government.Id, government.Name)).ToArray() ?? [];

    private static IReadOnlyCollection<LocationOptionViewModel> ToLocationOptions(IEnumerable<UiCityDto>? cities) =>
        cities?.Select(city => new LocationOptionViewModel(city.Id, city.Name)).ToArray() ?? [];

    private bool IsAjaxRequest() =>
        string.Equals(Request.Headers.XRequestedWith, "XMLHttpRequest", StringComparison.OrdinalIgnoreCase)
        || Request.Headers.Accept.Any(value => value?.Contains("application/json", StringComparison.OrdinalIgnoreCase) == true);

    private IActionResult AjaxValidationFailure() =>
        UnprocessableEntity(new CreateShipmentFlowResponse(
            false,
            "Please review the highlighted fields",
            "Please review the highlighted fields and try again.",
            null,
            null,
            ModelStateErrors()));

    private IActionResult AjaxApiFailure(ApiResponse response)
    {
        var messages = response.Errors.Count > 0
            ? response.Errors.Select(error => error.Message).Where(message => !string.IsNullOrWhiteSpace(message)).ToArray()
            : [ResolveApiMessage(response)];

        var isValidationFailure = response.Status == ResultStatus.ValidationError;
        var title = isValidationFailure ? "Please review the highlighted fields" : "Payment couldn't be completed";
        var message = isValidationFailure ? "Please review the highlighted fields and try again." : ResolveApiMessage(response);
        var validationErrors = isValidationFailure
            ? new Dictionary<string, string[]> { [string.Empty] = messages }
            : null;

        return StatusCode(
            response.StatusCode >= StatusCodes.Status400BadRequest ? response.StatusCode : StatusCodes.Status400BadRequest,
            new CreateShipmentFlowResponse(
                false,
                title,
                message,
                null,
                null,
                validationErrors));
    }

    private Dictionary<string, string[]> ModelStateErrors() =>
        ModelState
            .Where(item => item.Value?.ValidationState == ModelValidationState.Invalid)
            .ToDictionary(
                item => item.Key,
                item => item.Value?.Errors.Select(error => string.IsNullOrWhiteSpace(error.ErrorMessage) ? "Invalid value." : error.ErrorMessage).ToArray() ?? []);

    private static string ResolveApiMessage(ApiResponse response)
    {
        var message = response.Error?.Message
            ?? response.Errors.FirstOrDefault()?.Message
            ?? response.Message;

        if (string.IsNullOrWhiteSpace(message))
            return PaymentFailureFallbackMessage;

        return IsGenericMessage(message) ? PaymentFailureFallbackMessage : message;
    }

    private static bool IsGenericMessage(string message) =>
        string.Equals(message, "The request could not be completed.", StringComparison.OrdinalIgnoreCase)
        || string.Equals(message, "The request failed.", StringComparison.OrdinalIgnoreCase)
        || string.Equals(message, "Operation failed.", StringComparison.OrdinalIgnoreCase)
        || string.Equals(message, "Failed operation.", StringComparison.OrdinalIgnoreCase)
        || string.Equals(message, "An error occurred.", StringComparison.OrdinalIgnoreCase);
}

internal sealed record CreateShipmentFlowResponse(
    bool IsSuccess,
    string Title,
    string Message,
    UiInvoiceDto? Invoice,
    string? ShipmentDetailsUrl,
    Dictionary<string, string[]>? ValidationErrors);