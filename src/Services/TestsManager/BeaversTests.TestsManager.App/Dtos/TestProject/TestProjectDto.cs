namespace BeaversTests.TestsManager.App.Dtos.TestProject;

public class TestProjectDto
{
    public required Guid Id { get; init; }
    public required string UserCreatorId { get; init; }
    public required string Name { get; init; } = null!;
    public string? Description { get; init; }
}