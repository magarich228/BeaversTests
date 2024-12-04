namespace BeaversTests.Common.Binary;

public abstract class FileSystemEntity
{
    public required IEnumerable<BeaversTestsFile> Files { get; init; }
    public required IEnumerable<BeaversTestsDirectory> Directories { get; init; }
}