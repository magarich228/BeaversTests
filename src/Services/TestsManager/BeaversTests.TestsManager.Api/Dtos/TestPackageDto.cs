namespace BeaversTests.TestsManager.Api.Dtos;

public class TestPackageDto
{
    public required string Name { get; init; }
    public string? Description { get; init; }
    public required IFormFileCollection TestFiles { get; init; }
    public required IEnumerable<TestPackageDirectoryDto> Directories { get; init; }
    public string? TestPackageType { get; init; }
    public required Guid TestProjectId { get; init; }
}