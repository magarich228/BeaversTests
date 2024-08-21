namespace BeaversTests.TestsManager.App.Dtos;

public class NewTestPackageDirectoryDto
{
    public required string DirectoryName { get; init; }
    public required IEnumerable<TestPackageFileInfo> TestFiles { get; init; }
    public required IEnumerable<NewTestPackageDirectoryDto> Directories { get; init; }
}