using BeaversTests.TestsManager.Core.TestPackage;

namespace BeaversTests.TestsManager.Core.TestDriver;

public class TestDriver
{
    public required string Key { get; init; }
    public required string UserCreatorId { get; init; }
    public string? Description { get; set; }
    public IEnumerable<BeaversTestPackage>? TestPackages { get; init; }
}