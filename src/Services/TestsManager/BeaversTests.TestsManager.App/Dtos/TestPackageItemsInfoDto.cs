namespace BeaversTests.TestsManager.App.Dtos;

public class TestPackageItemsInfoDto
{
    public required Guid TestPackageId { get; init; }
    public required IEnumerable<TestPackageTestSuiteDto> TestSuites { get; init; }
}