using BeaversTests.Common.Binary;

namespace BeaversTests.TestRunner;

public class DriverValidationCommand : Command
{
    public string DriverKey { get; init; } = null!;
    public Guid AgId { get; init; }
    public TestDriverContent Driver { get; init; } = null!;
}