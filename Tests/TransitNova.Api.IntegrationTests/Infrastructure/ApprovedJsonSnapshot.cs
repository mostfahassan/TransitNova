using FluentAssertions;
using System.Text.Json;

namespace TransitNova.Api.IntegrationTests.Infrastructure;

internal static class ApprovedJsonSnapshot
{
    private const string ApprovalEnvironmentVariable = "TRANSITNOVA_APPROVE_CONTRACT_SNAPSHOTS";

    internal static async Task AssertMatchesAsync<T>(string relativePath, T snapshot)
    {
        var fullPath = Path.Combine(GetProjectRoot(), relativePath);
        Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);

        var actual = JsonSerializer.Serialize(snapshot, JsonOptions);

        if (string.Equals(
                Environment.GetEnvironmentVariable(ApprovalEnvironmentVariable),
                "1",
                StringComparison.Ordinal))
        {
            await File.WriteAllTextAsync(fullPath, actual);
        }

        File.Exists(fullPath).Should().BeTrue($"approved snapshot '{relativePath}' should exist");

        var expected = await File.ReadAllTextAsync(fullPath);
        actual.Should().Be(expected, $"snapshot '{relativePath}' changed");
    }

    private static string GetProjectRoot()
    {
        return Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
    }

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true
    };
}
