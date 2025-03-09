namespace BeaversTests.TestsManager.App.Abstractions;

public abstract class TestDriverBase
{
    public required string Key { get; init; }
    public string? Description { get; init; }
}