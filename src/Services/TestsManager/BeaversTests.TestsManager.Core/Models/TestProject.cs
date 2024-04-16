namespace BeaversTests.TestsManager.Core.Models;

public class TestProject
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
}