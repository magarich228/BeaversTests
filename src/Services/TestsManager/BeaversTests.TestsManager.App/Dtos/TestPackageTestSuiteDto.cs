namespace BeaversTests.TestsManager.App.Dtos;

public class TestPackageTestSuiteDto
{
    public required string Name { get; init; }
    public required IEnumerable<TestPackageTestDto> Tests { get; init; }
}