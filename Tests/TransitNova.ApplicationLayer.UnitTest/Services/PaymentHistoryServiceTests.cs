using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using System.Net;
using System.Text;
using TransitNova.BusinessLayer.DTOs.Payment;
using TransitNova.BusinessLayer.Options;
using TransitNova.BusinessLayer.Services.PaymentServices;
using TransitNova.Domain.Enums.Payment;

namespace TransitNova.ApplicationLayer.Tests.Services;

public sealed class PaymentHistoryServiceTests
{
    [Fact]
    public async Task GetPaymentHistoriesAsync_Should_BuildGetUrlWithQueryStringAndHeaderAsync()
    {
        var paymentId = Guid.Parse("44444444-4444-4444-4444-444444444444");
        using var service = CreatePaymentHistoryService(HttpStatusCode.OK, $$"""
        {
          "data": [
            {
              "id": 9,
              "paymentId": "{{paymentId}}",
              "paymentMethod": "CreditCard",
              "oldStatus": "Pending",
              "newStatus": "Success",
              "changedAt": "2026-07-10T09:30:00Z",
              "createdAt": "2026-07-10T09:00:00Z",
              "createdBy": "finance.audit"
            }
          ],
          "totalCount": 1,
          "pageNumber": 2,
          "pageSize": 5
        }
        """);

        var filter = new PaymentHistoryFilterDto
        {
            PaymentId = paymentId,
            PaymentStatus = PaymentStatus.Success,
            PaymentMethod = PaymentMethod.CreditCard,
            CreatedBy = "finance.audit",
            CreatedAtFrom = new DateTime(2026, 7, 1, 0, 0, 0, DateTimeKind.Utc),
            CreatedAtTo = new DateTime(2026, 7, 31, 23, 59, 59, DateTimeKind.Utc),
            PageNumber = 2,
            PageSize = 5
        };

        var result = await service.Instance.GetPaymentHistoriesAsync(filter, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.TotalCount.Should().Be(1);
        result.Data.PageNumber.Should().Be(2);
        result.Data.PageSize.Should().Be(5);
        result.Data.Data.Should().ContainSingle();
        result.Data.Data.Single().PaymentId.Should().Be(paymentId);
        service.Handler.Requests.Should().ContainSingle();
        service.Handler.Requests[0].Method.Should().Be(HttpMethod.Get);
        service.Handler.Requests[0].Headers.GetValues("X-PaymentKey").Should().ContainSingle("public-key");
        service.Handler.Requests[0].RequestUri!.ToString().Should().Contain("https://payments.test/api/v1/payments/history?");
        service.Handler.Requests[0].RequestUri!.Query.Should().Contain("PaymentId=44444444-4444-4444-4444-444444444444");
        service.Handler.Requests[0].RequestUri!.Query.Should().Contain("PaymentStatus=Success");
        service.Handler.Requests[0].RequestUri!.Query.Should().Contain("PaymentMethod=CreditCard");
        service.Handler.Requests[0].RequestUri!.Query.Should().Contain("CreatedBy=finance.audit");
        service.Handler.Requests[0].RequestUri!.Query.Should().Contain("PageNumber=2");
        service.Handler.Requests[0].RequestUri!.Query.Should().Contain("PageSize=5");
    }

    [Fact]
    public async Task GetPaymentHistoriesAsync_Should_ReturnFailure_WhenResponseIsMalformedAsync()
    {
        using var service = CreatePaymentHistoryService(HttpStatusCode.OK, "not-json");

        var result = await service.Instance.GetPaymentHistoriesAsync(new PaymentHistoryFilterDto(), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Message.Should().Be("Invalid payment history response");
    }

    [Fact]
    public async Task GetPaymentHistoriesAsync_Should_ReturnConfigurationFailure_WhenPublicKeyIsMissingAsync()
    {
        using var service = CreatePaymentHistoryService(HttpStatusCode.OK, "{}", publicKey: null);

        var result = await service.Instance.GetPaymentHistoriesAsync(new PaymentHistoryFilterDto(), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Message.Should().Be("Payment configuration missing");
        service.Handler.Requests.Should().BeEmpty();
    }

    [Fact]
    public async Task GetPaymentHistoriesAsync_Should_ReturnServiceUnreachable_WhenHttpClientThrowsAsync()
    {
        using var service = CreatePaymentHistoryService(new HttpRequestException("connection refused"));

        var result = await service.Instance.GetPaymentHistoriesAsync(new PaymentHistoryFilterDto(), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Message.Should().Be("Payment service unreachable");
    }

    private static TestPaymentHistoryService CreatePaymentHistoryService(
        HttpStatusCode statusCode,
        string content,
        string? publicKey = "public-key",
        string? baseUrl = "https://payments.test")
    {
        var handler = new StubHttpMessageHandler(_ => new HttpResponseMessage(statusCode)
        {
            Content = new StringContent(content, Encoding.UTF8, "application/json")
        });

        return CreatePaymentHistoryService(handler, publicKey, baseUrl);
    }

    private static TestPaymentHistoryService CreatePaymentHistoryService(Exception exception)
    {
        return CreatePaymentHistoryService(new StubHttpMessageHandler(_ => throw exception));
    }

    private static TestPaymentHistoryService CreatePaymentHistoryService(
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
        var service = new PaymentHistoryService(
            client,
            NullLogger<PaymentHistoryService>.Instance,
            options);

        return new TestPaymentHistoryService(service, handler, client);
    }

    private sealed class TestPaymentHistoryService(
        PaymentHistoryService instance,
        StubHttpMessageHandler handler,
        HttpClient client)
        : IDisposable
    {
        public PaymentHistoryService Instance { get; } = instance;
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
