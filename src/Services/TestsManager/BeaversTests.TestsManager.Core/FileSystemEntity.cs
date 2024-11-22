namespace BeaversTests.TestsManager.Core;

public abstract class FileSystemEntity
{
    public required IEnumerable<BeaversTestsFile> TestFiles { get; init; }
    public required IEnumerable<BeaversTestsDirectory> Directories { get; init; }
}