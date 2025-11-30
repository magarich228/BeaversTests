namespace BeaversTests.Drivers;

public class TestDriver
{
    public required string Key { get; init; }
    public required string UserCreatorId { get; init; } // TODO: Team ID?
    public string? Description { get; set; }
}