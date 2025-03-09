namespace BeaversTests.TestsManager.App.Dtos.TestDriver;

public class TestDriverDto
{
    public required string Key { get; init; }
    public required Guid AgId { get; init; }
    public string? Description { get; init; }
}