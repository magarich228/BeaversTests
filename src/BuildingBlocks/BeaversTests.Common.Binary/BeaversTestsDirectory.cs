namespace BeaversTests.Common.Binary;

public class BeaversTestsDirectory
{
    public required string DirectoryName { get; init; }
    public required IEnumerable<BeaversTestsFile> TestFiles { get; init; }
    public required IEnumerable<BeaversTestsDirectory> Directories { get; init; }
}