namespace BeaversTests.TestsManager.App.Dtos;

public class BeaversTestsDirectoryInfo
{
    public required string DirectoryName { get; init; }
    public required IEnumerable<BeaversTestsFileInfo> TestFiles { get; init; }
    public required IEnumerable<BeaversTestsDirectoryInfo> Directories { get; init; }
}