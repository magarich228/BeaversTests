namespace BeaversTests.TestsManager.Core.TestPackage;

public class BeaversTestPackage
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
    public required string TestDriverKey { get; init; }
    public required Guid TestProjectId { get; init; }
    public TestDriver.TestDriver? TestDriver { get; init; }
    public TestProject.TestProject? TestProject { get; init; }
}