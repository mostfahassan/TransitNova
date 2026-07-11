using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using TransitNova.Domain.Contracts.Roles;
using TransitNova.Domain.Enums.Result;
using TransitNova.UI.Infrastructure.Mvc.Common;
using TransitNova.UI.Infrastructure.Mvc.Interface;
using TransitNova.UI.ViewModels;
using TransitNovaUI.BusinessLayer.ApiContracts;
using TransitNovaUI.BusinessLayer.DTOs.Payment;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.User.Bundles.Segregation;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.User.Subscriptions.Segregation;

namespace TransitNova.UI.Areas.UserArea.Controllers;

[Authorize(Roles = Role.User)]
[Area("UserArea")]
[Route("[area]/[controller]/[action]")]
public sealed class SubscriptionsController(
    IBackendApiInvoker apiInvoker,
    IIdempotencyKeyFactory idempotencyKeyFactory,
    IUserBundlesQuery userBundlesQuery,
    IUserSubscriptionCommand userSubscriptionCommand)
    : AppControllerBase
{
    private const string PaymentFailureFallbackMessage = "No invoice was generated. Please review the payment method and try again.";

    [HttpGet("{bundleId:guid}")]
    public async Task<IActionResult> Checkout(Guid bundleId, CancellationToken cancellationToken)
    {
        var model = new BundleSubscriptionPaymentViewModel { BundleId = bundleId };
        var loaded = await PopulateBundleAsync(model, cancellationToken);
        return loaded ? View(model) : RedirectToAction("Index", "Dashboard", new { area = "UserArea" });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Checkout(BundleSubscriptionPaymentViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            await PopulateBundleAsync(model, cancellationToken);
            return IsAjaxRequest() ? AjaxValidationFailure() : View(model);
        }

        var response = await apiInvoker.ExecuteAsync((token, ct) => userSubscriptionCommand.SubscribeToBundleAsync(model.BundleId, model.ToDto(), token!, idempotencyKeyFactory.Create(), ct), cancellationToken: cancellationToken);

        if (response.IsFailure || response.Data is null)
        {
            AddApiErrors(response);
            if (IsAjaxRequest())
                return AjaxApiFailure(response);

            await PopulateBundleAsync(model, cancellationToken);
            return View(model);
        }

        if (IsAjaxRequest())
        {
            return StatusCode(
                response.StatusCode is >= 200 and < 300 ? response.StatusCode : StatusCodes.Status200OK,
                new BundleSubscriptionFlowResponse(
                    true,
                    "Bundle invoice ready",
                    "Your bundle payment was processed and the subscription is now active.",
                    response.Data,
                    null));
        }

        Success("Bundle subscription completed successfully.");
        return RedirectToAction("Index", "Dashboard", new { area = "UserArea" });
    }

    [HttpPost("{bundleId:guid}")]
    [ValidateAntiForgeryToken]
    public IActionResult Subscribe(Guid bundleId)
    {
        return RedirectToAction(nameof(Checkout), new { bundleId });
    }

    [HttpPost("{bundleId:guid}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Unsubscribe(Guid bundleId, CancellationToken cancellationToken)
    {
        var response = await apiInvoker.ExecuteAsync((token, ct) => userSubscriptionCommand.UnsubscribeFromBundleAsync(bundleId, token!, idempotencyKeyFactory.Create(), ct), cancellationToken: cancellationToken);

        if (response.IsFailure)
            ApiError(response);
        else
            Success("Subscription cancelled successfully.");

        return RedirectToAction("Index", "Profile", new { area = "UserArea" });
    }

    private async Task<bool> PopulateBundleAsync(BundleSubscriptionPaymentViewModel model, CancellationToken cancellationToken)
    {
        var response = await apiInvoker.ExecuteAsync((token, ct) => userBundlesQuery.GetUserBundleByIdAsync(model.BundleId, token!, ct), cancellationToken: cancellationToken);
        if (response.IsFailure || response.Data is null)
        {
            ApiError(response);
            return false;
        }

        model.Bundle = response.Data;
        return true;
    }

    private bool IsAjaxRequest() =>
        string.Equals(Request.Headers.XRequestedWith, "XMLHttpRequest", StringComparison.OrdinalIgnoreCase)
        || Request.Headers.Accept.Any(value => value?.Contains("application/json", StringComparison.OrdinalIgnoreCase) == true);

    private IActionResult AjaxValidationFailure() =>
        UnprocessableEntity(new BundleSubscriptionFlowResponse(
            false,
            "Please review the highlighted fields",
            "Please review the highlighted fields and try again.",
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
            new BundleSubscriptionFlowResponse(
                false,
                title,
                message,
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

internal sealed record BundleSubscriptionFlowResponse(
    bool IsSuccess,
    string Title,
    string Message,
    UiBundleInvoiceDto? Invoice,
    Dictionary<string, string[]>? ValidationErrors);
