namespace BeaversTests.TestDrivers.Models;

// TODO: Подумать над вложенностью
public class TestSuite
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required IEnumerable<Test> Tests { get; init; }
}