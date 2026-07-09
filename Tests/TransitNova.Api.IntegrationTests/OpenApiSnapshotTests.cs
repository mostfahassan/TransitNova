using VerifyXunit;
using TransitNova.Api.IntegrationTests.Infrastructure;
using Xunit;

namespace TransitNova.Api.IntegrationTests
{
    public class OpenApiSnapshotTests : IClassFixture<TransitNovaWebApplicationFactory>, IAsyncLifetime
    {
        private readonly TransitNovaWebApplicationFactory _factory;

        public OpenApiSnapshotTests(TransitNovaWebApplicationFactory factory)
        {
            _factory = factory;
        }

        public Task InitializeAsync() => _factory.InitializeDatabaseAsync();

        public Task DisposeAsync() => Task.CompletedTask;

        [Fact]
        public async Task OpenApi_Contract_Should_Not_Change()
        {
            var client = _factory.CreateAnonymousClient();

            var response = await client.GetAsync("/openapi/v1.json");
            response.EnsureSuccessStatusCode();

            var currentSwaggerJson = await response.Content.ReadAsStringAsync();

            await Verifier.Verify(currentSwaggerJson);
        }
    }
}
