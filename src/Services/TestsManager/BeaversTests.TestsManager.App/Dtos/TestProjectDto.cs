namespace BeaversTests.TestsManager.App.Dtos;

public class TestProjectDto
{
    public required Guid Id { get; init; }
    public required string Name { get; init; } = null!;
    public string? Description { get; init; }
}