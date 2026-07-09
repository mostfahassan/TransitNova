using System.Net.Http.Json;
using System.Threading;
using TransitNova.BusinessLayer.DTOs.Shipment;
using TransitNova.BusinessLayer.DTOs.UserProfile.Auth;
using TransitNova.Domain.Enums.Shipment;
using TransitNova.Domain.Enums.Users;

namespace TransitNova.Api.IntegrationTests.Infrastructure;

internal static class EndpointRequestFactory
{
    private static int _registerSequence;

    internal static async Task<HttpRequestMessage> CreateRequestAsync(
        TransitNovaWebApplicationFactory factory,
        ControllerEndpoint endpoint,
        string idempotencyKey,
        CancellationToken cancellationToken = default)
    {
        var request = new HttpRequestMessage(new HttpMethod(endpoint.HttpMethod), endpoint.RequestPath);

        if (endpoint.HttpMethod is "POST" or "PUT" or "PATCH" or "DELETE")
            request.Headers.Add("X-Idempotency-Key", idempotencyKey);

        var content = await CreateContentAsync(factory, endpoint, cancellationToken);
        if (content is not null)
            request.Content = content;

        return request;
    }

    private static async Task<HttpContent?> CreateContentAsync(
        TransitNovaWebApplicationFactory factory,
        ControllerEndpoint endpoint,
        CancellationToken cancellationToken)
    {
        if (endpoint.HttpMethod is not ("POST" or "PUT" or "PATCH"))
            return null;

        object payload = endpoint.EndpointName switch
        {
            "Register User" => await BuildRegisterDtoAsync(factory, cancellationToken),
            "Login User" => new LoginDto(TransitNovaWebApplicationFactory.KnownUserPassword, TransitNovaWebApplicationFactory.KnownUserEmail),
            "Calculate Shipment Rate" => new RateCalculatorDto(
                new PackageSpecificationDto
                {
                    Weight = 2.5m,
                    Width = 25m,
                    Height = 10m,
                    Length = 35m
                },
                TransportationMode.Land,
                enShipmentType.Standard),
            _ => new { }
        };

        return JsonContent.Create(payload);
    }

    private static async Task<RegisterDto> BuildRegisterDtoAsync(
        TransitNovaWebApplicationFactory factory,
        CancellationToken cancellationToken)
    {
        _ = cancellationToken;
        var cityId = await factory.GetAnyCityIdAsync();
        var sequence = Interlocked.Increment(ref _registerSequence);

        return new RegisterDto
        {
            UserName = $"contractuser{sequence}",
            Email = $"contract-user-{sequence}@transitnova.test",
            Password = TransitNovaWebApplicationFactory.KnownUserPassword,
            ConfirmPassword = TransitNovaWebApplicationFactory.KnownUserPassword,
            PhoneNumber = $"+20100{sequence:0000000}",
            FirstName = "Contract",
            LastName = "User",
            Address = "Integration Address",
            UserType = UserType.User,
            CityId = cityId
        };
    }
}
