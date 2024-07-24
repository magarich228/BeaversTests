namespace BeaversTests.TestsManager.Api.Dtos;

public class TestPackageDto
{
    public required string Name { get; init; }
    public string? Description { get; init; }
    public required IFormFileCollection TestAssemblies { get; init; }
    public required IEnumerable<string> ItemPaths { get; init; }
    public string? TestPackageType { get; init; }
    public required Guid TestProjectId { get; init; }
}