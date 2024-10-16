namespace BeaversTests.TestsManager.App.Abstractions;

public abstract class TestPackageBase
{
    public required string Name { get; init; }
    public string? Description { get; init; }
    public required string TestPackageType { get; init; }
    public required Guid TestProjectId { get; init; }
}