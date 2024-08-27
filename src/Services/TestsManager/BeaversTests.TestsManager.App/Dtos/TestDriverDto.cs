namespace BeaversTests.TestsManager.App.Dtos;

public class TestDriverDto
{
    public required string Key { get; init; }
    public bool IsDefault { get; init; }
    public string? Description { get; init; }
}