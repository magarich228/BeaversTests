using BeaversTests.TestsManager.Core.TestPackage;

namespace BeaversTests.TestsManager.Core.TestDriver;

public class TestDriver
{
    public required Guid AgId { get; init; }
    public required string Key { get; init; }
    public required string UserCreatorId { get; init; }
    public ValidationResult ValidationResult { get; set; }
    public string? ValidationMessage { get; set; }
    public string? Description { get; set; }
    public IEnumerable<BeaversTestPackage>? TestPackages { get; init; }
}