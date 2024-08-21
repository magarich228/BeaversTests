namespace BeaversTests.TestsManager.App.Dtos;

public class NewTestPackageDto
{
    public required string Name { get; init; }
    public string? Description { get; init; }
    public required IEnumerable<TestPackageFileInfo> TestFiles { get; init; }
    public required IEnumerable<NewTestPackageDirectoryDto> Directories { get; init; }
    public string? TestPackageType { get; init; }
    public required Guid TestProjectId { get; init; }
}