namespace BeaversTests.CLI;

public class NewTestPackageFromDirectoryDto
{
    public required string Name { get; init; }
    public string? Description { get; init; }
    public required DirectoryInfo Directory { get; init; }
    public required string TestPackageType { get; init; }
    public required Guid TestProjectId { get; init; }
}