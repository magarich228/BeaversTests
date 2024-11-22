namespace BeaversTests.TestsManager.App.Dtos;

public class EntityContentDto
{
    public required IEnumerable<BeaversTestsFileInfo> TestFiles { get; init; }
    public required IEnumerable<BeaversTestsDirectoryInfo> Directories { get; init; }
}