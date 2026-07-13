namespace TransitNova.E2E.Tests;

[CollectionDefinition(Name, DisableParallelization = true)]
public sealed class E2ECollection : ICollectionFixture<TransitNovaBrowserFixture>
{
    public const string Name = "TransitNova E2E";
}
