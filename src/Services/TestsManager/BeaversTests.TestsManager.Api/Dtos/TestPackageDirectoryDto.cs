namespace BeaversTests.TestsManager.Api.Dtos;

public class TestPackageDirectoryDto
{
    public required string DirectoryName { get; init; }
    public required IFormFileCollection TestFiles { get; init; }
    public required IEnumerable<TestPackageDirectoryDto> Directories { get; init; }
}