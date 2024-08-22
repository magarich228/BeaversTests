namespace BeaversTests.TestsManager.Api.Dtos;

public class TestPackageDto
{
    public required string Name { get; init; }
    public string? Description { get; init; }
    public required TestPackageContentDto Content { get; init; }
    public required string TestPackageType { get; init; }
    public required Guid TestProjectId { get; init; }
}

public class TestPackageContentDto
{
    public required IFormFileCollection TestFiles { get; init; }
    public required IEnumerable<TestPackageDirectoryDto> Directories { get; init; }
}