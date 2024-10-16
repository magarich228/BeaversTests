namespace BeaversTests.TestsManager.App.Dtos;

public class NewTestPackageDto
{
    public required string Name { get; init; }
    public string? Description { get; init; }
    public required NewTestPackageContentDto Content { get; init; }
    public required string TestDriver { get; init; }
    public required Guid TestProjectId { get; init; }
}

public class NewTestPackageContentDto
{
    public required IEnumerable<NewTestPackageFileInfo> TestFiles { get; init; }
    public required IEnumerable<NewTestPackageDirectoryInfo> Directories { get; init; }
}