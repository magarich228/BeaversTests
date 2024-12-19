namespace BeaversTests.TestRunnerAgent.Core.Tasks;

public class DriverValidationTask : ITask
{
    public required string DriverKey { get; init; }
    public required Guid AgId { get; init; }
    public TaskType Type => TaskType.DriverValidation;
}