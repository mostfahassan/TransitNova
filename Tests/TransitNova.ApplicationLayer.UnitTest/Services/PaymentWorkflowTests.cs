using FluentAssertions;
using FluentValidation;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using System.Net;
using System.Text;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Payment;
using TransitNova.BusinessLayer.Interfaces.Services.PaymentService;
using TransitNova.BusinessLayer.Options;
using TransitNova.BusinessLayer.Services.PaymentServices;
using TransitNova.BusinessLayer.Validators.PaymentValidators;
using TransitNova.Domain.Enums.Payment;

namespace TransitNova.ApplicationLayer.Tests.Services;

public sealed class PaymentWorkflowTests
{
    [Fact]
    public async Task PaymentService_Should_MapSuccessfulGatewayEnvelopeToInvoiceAsync()
    {
        var shipmentId = Guid.NewGuid();
        var paymentId = Guid.NewGuid();
        using var service = CreatePaymentService(HttpStatusCode.OK, $$"""
        {
          "isSuccess": true,
          "isFailure": false,
          "statusCode": 200,
          "message": "PaymentProcess completed successfully.",
          "data": {
            "paymentId": "{{paymentId}}",
            "shipmentId": "{{shipmentId}}",
            "amount": 125,
            "commission": 12.5,
            "totalAmount": 137.5,
            "paymentMethod": "CreditCard",
            "status": "Success",
            "paidAt": "2026-06-27T00:00:00Z",
            "notes": null
          }
        }
        """);
        var dto = CreatePaymentRequest(shipmentId, 125m);

        var result = await service.Instance.Pay(dto, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.PaymentId.Should().Be(paymentId);
        result.Data.ShipmentId.Should().Be(shipmentId);
        result.Data.ShippingCost.Should().Be(125m);
        result.Data.Commission.Should().Be(12.5m);
        result.Data.Amount.Should().Be(137.5m);
        result.Data.Status.Should().Be("Success");
        service.Handler.Requests.Should().ContainSingle();
        service.Handler.Requests[0].Headers.GetValues("X-PaymentKey").Should().ContainSingle("public-key");
        service.Handler.Requests[0].RequestUri!.ToString().Should().Be("https://payments.test/api/v1/payments/pay");
    }

    [Fact]
    public async Task PaymentService_Should_ReturnFailure_WhenGatewayEnvelopeIsFailureAsync()
    {
        using var service = CreatePaymentService(HttpStatusCode.Unauthorized, """
        {
          "isSuccess": false,
          "isFailure": true,
          "statusCode": 401,
          "message": "PaymentProcess authentication failed.",
          "error": { "message": "Invalid payment gateway authentication key." }
        }
        """);

        var result = await service.Instance.Pay(CreatePaymentRequest(Guid.NewGuid(), 125m), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Message.Should().Be("Invalid payment gateway authentication key.");
    }

    [Fact]
    public async Task PaymentService_Should_ReturnFailure_WhenGatewayReportsFailedTransactionAsync()
    {
        var shipmentId = Guid.NewGuid();
        var paymentId = Guid.NewGuid();
        using var service = CreatePaymentService(HttpStatusCode.OK, $$"""
        {
          "isSuccess": true,
          "isFailure": false,
          "statusCode": 200,
          "data": {
            "paymentId": "{{paymentId}}",
            "shipmentId": "{{shipmentId}}",
            "amount": 125,
            "commission": 12.5,
            "totalAmount": 137.5,
            "paymentMethod": "CreditCard",
            "status": "Failed",
            "notes": "Transaction declined by the issuing bank."
          }
        }
        """);

        var result = await service.Instance.Pay(CreatePaymentRequest(shipmentId, 125m), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Message.Should().Be("Transaction declined by the issuing bank.");
    }

    [Fact]
    public async Task PaymentService_Should_ReturnInvalidResponse_WhenGatewayReturnsMalformedJsonAsync()
    {
        using var service = CreatePaymentService(HttpStatusCode.OK, "not-json");

        var result = await service.Instance.Pay(CreatePaymentRequest(Guid.NewGuid(), 125m), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Message.Should().Be("Invalid payment response");
    }

    [Fact]
    public async Task PaymentService_Should_ReturnConfigurationFailure_WhenPublicKeyIsMissingAsync()
    {
        using var service = CreatePaymentService(HttpStatusCode.OK, "{}", publicKey: null);

        var result = await service.Instance.Pay(CreatePaymentRequest(Guid.NewGuid(), 125m), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Message.Should().Be("Payment configuration missing");
        service.Handler.Requests.Should().BeEmpty();
    }

    [Fact]
    public async Task PaymentService_Should_ReturnServiceUnreachable_WhenHttpClientThrowsAsync()
    {
        using var service = CreatePaymentService(new HttpRequestException("connection refused"));

        var result = await service.Instance.Pay(CreatePaymentRequest(Guid.NewGuid(), 125m), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Message.Should().Be("Payment service unreachable");
    }

  

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(100001)]
    public async Task CreatePaymentDtoValidator_Should_RejectInvalidShippingCostsAsync(decimal shippingCost)
    {
        var validator = new CratePaymentDtoValidator();

        var result = await validator.ValidateAsync(CreatePaymentRequest(Guid.NewGuid(), shippingCost));

        result.IsValid.Should().BeFalse();
    }

    private static CreatePaymentDto CreatePaymentRequest(Guid shipmentId, decimal shippingCost)
    {
        return new CreatePaymentDto
        {
            ShipmentId = shipmentId,
            PaymentMethod = PaymentMethod.CreditCard,
            ShippingCost = shippingCost
        };
    }

    private static TestPaymentService CreatePaymentService(
        HttpStatusCode statusCode,
        string content,
        string? publicKey = "public-key",
        string? baseUrl = "https://payments.test")
    {
        var handler = new StubHttpMessageHandler(_ => new HttpResponseMessage(statusCode)
        {
            Content = new StringContent(content, Encoding.UTF8, "application/json")
        });

        return CreatePaymentService(handler, publicKey, baseUrl);
    }

    private static TestPaymentService CreatePaymentService(Exception exception)
    {
        return CreatePaymentService(new StubHttpMessageHandler(_ => throw exception));
    }

    private static TestPaymentService CreatePaymentService(
        StubHttpMessageHandler handler,
        string? publicKey = "public-key",
        string? baseUrl = "https://payments.test")
    {
        var options = Options.Create(new PaymentSettings
        {
            PublicKey = publicKey,
            BaseUrl = baseUrl
        });

        var client = new HttpClient(handler);
        var service = new PaymentService(
            client,
            NullLogger<PaymentService>.Instance,
            options);

        return new TestPaymentService(service, handler, client);
    }

    private sealed class TestPaymentService(PaymentService instance, StubHttpMessageHandler handler, HttpClient client) : IDisposable
    {
        public PaymentService Instance { get; } = instance;
        public StubHttpMessageHandler Handler { get; } = handler;

        public void Dispose()
        {
            client.Dispose();
        }
    }

    private sealed class StubHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> send) : HttpMessageHandler
    {
        public List<HttpRequestMessage> Requests { get; } = [];

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Requests.Add(request);
            return Task.FromResult(send(request));
        }
    }
}
