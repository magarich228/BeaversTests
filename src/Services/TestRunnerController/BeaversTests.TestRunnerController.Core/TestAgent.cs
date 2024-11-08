namespace BeaversTests.TestRunnerController.Core;

public class TestAgent
{
    public Guid Id { get; init; }
    public TestAgentStatus Status { get; set; } = TestAgentStatus.Created;
}