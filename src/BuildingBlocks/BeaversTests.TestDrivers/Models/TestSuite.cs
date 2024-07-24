namespace BeaversTests.TestDrivers.Models;

public class TestSuite
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required IEnumerable<Test> Tests { get; init; }
}