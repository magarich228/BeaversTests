namespace BeaversTests.TestsManager.App.Dtos.TestPackage;

public class TestPackageItemsInfoDto
{
    public required Guid TestPackageId { get; init; }
    public required IEnumerable<TestPackageTestSuiteDto> TestSuites { get; init; }
}