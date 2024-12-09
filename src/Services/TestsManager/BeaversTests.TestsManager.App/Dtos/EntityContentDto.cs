namespace BeaversTests.TestsManager.App.Dtos;

public class EntityContentDto
{
    public required IEnumerable<BeaversTestsFileInfo> Files { get; init; }
    public required IEnumerable<BeaversTestsDirectoryInfo> Directories { get; init; }
}