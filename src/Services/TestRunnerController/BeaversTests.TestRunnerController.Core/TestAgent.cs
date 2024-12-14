namespace BeaversTests.TestRunnerController.Core;

public class TestAgent
{
    public Guid Id { get; init; }
    public string OwnerId { get; init; } = null!;
    public string Key { get; init; } = null!;
    public TestAgentStatus Status { get; set; } = TestAgentStatus.Created;
}