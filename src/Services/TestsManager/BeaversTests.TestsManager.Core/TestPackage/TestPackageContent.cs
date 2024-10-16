namespace BeaversTests.TestsManager.Core.TestPackage;

// TODO: add ref to BeaversTestPackage??
public class TestPackageContent
{
    public required IEnumerable<TestPackageFile> TestFiles { get; init; }
    public required IEnumerable<TestPackageContentDirectory> Directories { get; init; }
}