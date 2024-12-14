namespace BeaversTests.TestRunnerController.App.Dtos;

public class AgentConnectionKeyDto
{
    public required Guid Id { get; init; }
    public required string Key { get; init; }
    public required string OwnerId { get; init; }
}