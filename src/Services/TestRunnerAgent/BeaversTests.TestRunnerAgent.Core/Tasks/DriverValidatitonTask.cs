namespace BeaversTests.TestRunnerAgent.Core.Tasks;

public class DriverValidationTask
{
    public required string DriverKey { get; init; }
    public required Guid AgId { get; init; }
}