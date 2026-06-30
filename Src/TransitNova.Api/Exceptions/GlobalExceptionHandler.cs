using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TransitNova.Domain.DomainExceptions;
namespace TransitNova.Api.Exceptions;
internal sealed class GlobalExceptionHandler(
    IProblemDetailsService problemDetailsService) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        httpContext.Response.ContentType = "application/problem+json";
        httpContext.Response.StatusCode = GetStatusCode(exception);

        var problemDetails = BuildProblemDetails(exception, httpContext);
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
            ValidationException => StatusCodes.Status400BadRequest,
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

            // --- Conflicts (409 Conflict) ---
            DuplicateShipmentInTripException => StatusCodes.Status409Conflict,
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

    private static ProblemDetails BuildProblemDetails(Exception exception, HttpContext httpContext)
    {
        var problemDetails = new ProblemDetails
        {
            Type = $"https://api.transitnova.com/errors/{exception.GetType().Name}",
            Title = exception.Message,
            Status = httpContext.Response.StatusCode,
            Detail = GetExceptionDetails(
                exception,
                httpContext)
        };

        EnrichProblemDetails(problemDetails, exception);
        return problemDetails;
    }

    private static string? GetExceptionDetails(Exception exception, HttpContext httpContext)
    {
        var environment = httpContext.RequestServices.GetRequiredService<IWebHostEnvironment>();
        return environment.IsDevelopment()
            ? exception.StackTrace
            : null;
    }

    private static void EnrichProblemDetails(ProblemDetails problemDetails, Exception exception)
    {
        AddDomainExceptionMetadata(problemDetails, exception);

        AddValidationErrors(problemDetails, exception);
        problemDetails.Extensions["traceId"] = Activity.Current?.Id ?? Guid.NewGuid().ToString();
    }

    private static void AddDomainExceptionMetadata(ProblemDetails problemDetails, Exception exception)
    {
        if (exception is not DomainException domainException)
            return;

        problemDetails.Extensions["errorCode"] = domainException.ErrorCode;

        if (domainException.EntityType is not null)
        {
            problemDetails.Extensions["entityType"] =
                domainException.EntityType;
        }

        if (domainException.EntityId is not null)
        {
            problemDetails.Extensions["entityId"] =
                domainException.EntityId;
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
