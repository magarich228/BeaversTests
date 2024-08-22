namespace BeaversTests.TestsManager.App.Dtos;

public class NewTestPackageDirectoryDto
{
    public required string DirectoryName { get; init; }
    public required IEnumerable<NewTestPackageFileInfo> TestFiles { get; init; }
    public required IEnumerable<NewTestPackageDirectoryDto> Directories { get; init; }
}