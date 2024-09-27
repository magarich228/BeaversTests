namespace BeaversTests.TestsManager.App.Dtos;

public class NewTestPackageDirectoryInfo
{
    public required string DirectoryName { get; init; }
    public required IEnumerable<NewTestPackageFileInfo> TestFiles { get; init; }
    public required IEnumerable<NewTestPackageDirectoryInfo> Directories { get; init; }
}