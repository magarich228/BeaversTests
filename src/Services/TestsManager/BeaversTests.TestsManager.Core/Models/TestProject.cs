namespace BeaversTests.TestsManager.Core.Models;

public class TestProject
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
}