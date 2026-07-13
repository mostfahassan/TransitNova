using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TransitNova.Domain.DomainExceptions;
using TransitNova.InfraStructure.Common.Exceptions;

namespace TransitNova.Api.Exceptions;

internal sealed class GlobalExceptionHandler(
    IProblemDetailsService problemDetailsService,
    ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        httpContext.Response.ContentType = "application/problem+json";
        httpContext.Response.StatusCode = GetStatusCode(exception);

        var traceId = Activity.Current?.Id ?? httpContext.TraceIdentifier;
        logger.LogError(
            exception,
            "Request failed with status code {StatusCode}. TraceId: {TraceId}",
            httpContext.Response.StatusCode,
            traceId);

        var problemDetails = BuildProblemDetails(exception, httpContext, traceId);
        return await problemDetailsService
            .TryWriteAsync(new ProblemDetailsContext
            {
                HttpContext = httpContext,
                ProblemDetails = problemDetails
            });
    }

    private static int GetStatusCode(Exception exception)
    {
        return exception switch
        {
            // --- System & Validations ---
            ValidationException => StatusCodes.Status422UnprocessableEntity,
            UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
            KeyNotFoundException => StatusCodes.Status404NotFound,
            TimeoutException => StatusCodes.Status504GatewayTimeout,

            // --- Not Found Exceptions ---
            EntityNotFoundException => StatusCodes.Status404NotFound,
            NotFoundException => StatusCodes.Status404NotFound,

            // --- Domain Logic & Business Rules (400 Bad Request) ---
            InvalidShipmentStateException => StatusCodes.Status400BadRequest,
            TripPlanningException => StatusCodes.Status400BadRequest,
            DomainArgumentException => StatusCodes.Status400BadRequest,
            DomainArgumentOutOfRangeException => StatusCodes.Status400BadRequest,
            DomainOperationException => StatusCodes.Status400BadRequest,
            InvalidCarrierStatusException => StatusCodes.Status400BadRequest,
            ShipmentNotAssignedException => StatusCodes.Status400BadRequest,

            // --- Identity operations ---
            UserCreationException userCreationException when userCreationException.IsDuplicate
                => StatusCodes.Status409Conflict,
            UserCreationException => StatusCodes.Status422UnprocessableEntity,

            // --- Conflicts (409 Conflict) ---
            DuplicateShipmentInTripException => StatusCodes.Status409Conflict,
            ConflictRequestException => StatusCodes.Status409Conflict,
            SameWarehouseManagerException => StatusCodes.Status409Conflict,
            WarehouseAlreadyHasManagerException => StatusCodes.Status409Conflict,

            // --- Unprocessable Entity (422) ---
            VehicleCapacityExceededException => StatusCodes.Status422UnprocessableEntity,

            // --- Authentication / Tokens ---
            ReusedRefreshTokenException => StatusCodes.Status403Forbidden,
            InvalidRefreshTokenException => StatusCodes.Status401Unauthorized,
            RefreshTokenNotFoundException => StatusCodes.Status401Unauthorized,
            RevokingRefreshTokenException => StatusCodes.Status400BadRequest,

            // --- Fallback ---
            _ => StatusCodes.Status500InternalServerError
        };
    }

    private static ProblemDetails BuildProblemDetails(Exception exception, HttpContext httpContext, string traceId)
    {
        var statusCode = httpContext.Response.StatusCode;
        var problemDetails = new ProblemDetails
        {
            Type = $"https://api.transitnova.com/errors/{exception.GetType().Name}",
            Title = GetTitle(exception, statusCode),
            Status = statusCode,
            Detail = GetExceptionDetails(exception, httpContext)
        };

        EnrichProblemDetails(problemDetails, exception, traceId);
        return problemDetails;
    }

    private static string GetTitle(Exception exception, int statusCode)
    {
        return statusCode switch
        {
            StatusCodes.Status500InternalServerError => "An unexpected server error occurred.",
            StatusCodes.Status422UnprocessableEntity when exception is ValidationException => "Validation failed.",
            StatusCodes.Status422UnprocessableEntity when exception is UserCreationException => "User creation failed.",
            StatusCodes.Status401Unauthorized => "Authentication failed.",
            StatusCodes.Status403Forbidden => "Access denied.",
            StatusCodes.Status404NotFound => "Resource not found.",
            StatusCodes.Status409Conflict => "Request conflict.",
            StatusCodes.Status504GatewayTimeout => "The request timed out.",
            _ => exception.Message
        };
    }

    private static string? GetExceptionDetails(Exception exception, HttpContext httpContext)
    {
        var environment = httpContext.RequestServices.GetRequiredService<IWebHostEnvironment>();
        return environment.IsDevelopment()
            ? exception.StackTrace
            : null;
    }

    private static void EnrichProblemDetails(ProblemDetails problemDetails, Exception exception, string traceId)
    {
        AddDomainExceptionMetadata(problemDetails, exception);
        AddValidationErrors(problemDetails, exception);
        problemDetails.Extensions["traceId"] = traceId;
    }

    private static void AddDomainExceptionMetadata(ProblemDetails problemDetails, Exception exception)
    {
        if (exception is not DomainException domainException)
            return;

        problemDetails.Extensions["errorCode"] = domainException.ErrorCode;

        if (domainException.EntityType is not null)
        {
            problemDetails.Extensions["entityType"] = domainException.EntityType;
        }

        if (domainException.EntityId is not null)
        {
            problemDetails.Extensions["entityId"] = domainException.EntityId;
        }
    }

    private static void AddValidationErrors(ProblemDetails problemDetails, Exception exception)
    {
        if (exception is not ValidationException validationException)
            return;

        problemDetails.Extensions["errors"] = validationException.Errors
                            .GroupBy(x => x.PropertyName)
                            .ToDictionary(g => g.Key,
                                          g => g.Select(x => x.ErrorMessage)
                                                .ToArray());
    }
}

