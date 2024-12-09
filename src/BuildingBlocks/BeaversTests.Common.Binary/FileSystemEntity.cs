namespace BeaversTests.Common.Binary;

public abstract class FileSystemEntity
{
    public IEnumerable<BeaversTestsFile> Files { get; init; } = new List<BeaversTestsFile>();
    public IEnumerable<BeaversTestsDirectory> Directories { get; init; } = new List<BeaversTestsDirectory>();
}