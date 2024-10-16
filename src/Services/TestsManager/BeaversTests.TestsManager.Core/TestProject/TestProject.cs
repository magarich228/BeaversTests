using BeaversTests.TestsManager.Core.TestPackage;

namespace BeaversTests.TestsManager.Core.TestProject;

public class TestProject
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
    public IEnumerable<BeaversTestPackage>? TestPackages { get; init; }
}