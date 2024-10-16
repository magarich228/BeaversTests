using BeaversTests.TestsManager.Core.TestPackage;

namespace BeaversTests.TestsManager.Core.TestDriver;

// TODO: add user id
// TODO: rename to plugin
public class TestDriver
{
    public required string Key { get; init; }
    public bool IsDefault { get; init; } // TODO: IsLocal
    public string? Description { get; init; }
    public IEnumerable<BeaversTestPackage>? TestPackages { get; init; }
}