namespace BeaversTests.TestsManager.Core.Models;

// TODO: rename to plugin
public class TestDriver
{
    public required string Key { get; init; }
    public string? Description { get; init; }
    public IEnumerable<BeaversTestPackage>? TestPackages { get; init; }
}